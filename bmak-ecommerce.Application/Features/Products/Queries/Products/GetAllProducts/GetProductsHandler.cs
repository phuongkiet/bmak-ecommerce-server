using AutoMapper;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Domain.Models;
using bmak_ecommerce.Application.Features.Products.DTOs.Catalog;
using bmak_ecommerce.Domain.Entities.Catalog;
using bmak_ecommerce.Domain.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using Microsoft.EntityFrameworkCore;
using bmak_ecommerce.Application.Common.Attributes;

namespace bmak_ecommerce.Application.Features.Products.Queries.Products.GetAllProducts
{
    [AutoRegister]

    public class GetProductsHandler : IQueryHandler<GetProductsQuery, ProductListResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetProductsHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result<ProductListResponse>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var specParams = request.Params;

            // 1. Khởi tạo Query (Đã filter theo search, category, price...)
            // QUAN TRỌNG: Query này chưa Skip/Take
            var query = _unitOfWork.Repository<Product>().GetAllAsQueryable()
                .Include(p => p.Category)
                .Include(p => p.AttributeValues)
                .ThenInclude(av => av.Attribute) // Include sâu để lấy tên Attribute
                .Where(x => x.IsActive && !x.IsDeleted); // Filter mặc định

            // --- ÁP DỤNG CÁC FILTER TỪ REQUEST (Search, MinPrice...) ---
            // (Giữ nguyên logic filter của bạn ở đây)
            if (!string.IsNullOrEmpty(specParams.Search))
            {
                var search = specParams.Search.ToLower();
                query = query.Where(x => x.Name.ToLower().Contains(search));
            }

            // Lưu ý: Khi tính Filter động, thường người ta KHÔNG áp dụng Filter của chính attribute đó 
            // để tránh việc chọn "Màu đỏ" xong thì mất luôn option "Màu xanh".
            // Nhưng để đơn giản, ở đây mình cứ dùng query đã filter full nhé.

            // ==========================================================
            // TÍNH TOÁN DỮ LIỆU BỘ LỌC (AGGREGATION)
            // ==========================================================

            var filterResponse = new ProductFilterAggregationDto();

            // Chỉ tính toán nếu có kết quả
            var totalCount = await query.CountAsync(cancellationToken);

            if (totalCount > 0)
            {
                // 1. Tính Min/Max Price thực tế của tập kết quả
                // Dùng GroupBy(1) để gom lại tính 1 lần cho nhanh
                var priceStats = await query
                    .Select(x => x.SalePrice > 0 ? x.SalePrice : x.BasePrice)
                    .GroupBy(x => 1)
                    .Select(g => new
                    {
                        Min = g.Min(),
                        Max = g.Max()
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                filterResponse.MinPrice = priceStats?.Min ?? 0;
                filterResponse.MaxPrice = priceStats?.Max ?? 0;

                // 2. Thống kê Danh mục (Category Facet)
                // Đếm xem trong kết quả tìm kiếm, mỗi danh mục có bao nhiêu sản phẩm
                var categoryStats = await query
                    .GroupBy(x => new { x.CategoryId, x.Category.Name })
                    .Select(g => new FilterItemDto
                    {
                        Value = g.Key.CategoryId.ToString(),
                        Label = g.Key.Name,
                        Count = g.Count()
                    })
                    .ToListAsync(cancellationToken);

                filterResponse.Categories.Add(new FilterGroupDto
                {
                    Code = "CATEGORY",
                    Name = "Danh mục",
                    Options = categoryStats
                });

                // 3. Thống kê Thuộc tính (Attribute Facet - Màu, Size...)
                // Logic: Flatten danh sách sản phẩm ra thành danh sách AttributeValue -> Group lại
                var attributeStats = await query
                    .SelectMany(x => x.AttributeValues) // Bung nhỏ ra từng dòng value
                    .GroupBy(x => new { x.Attribute.Code, x.Attribute.Name, x.Value }) // Group theo Loại + Giá trị
                    .Select(g => new
                    {
                        Code = g.Key.Code,
                        Name = g.Key.Name,
                        Value = g.Key.Value,
                        Count = g.Count()
                    })
                    .ToListAsync(cancellationToken);

                // Map kết quả group phẳng ở trên vào cấu trúc lồng nhau (Group -> Options)
                var groupedAttributes = attributeStats
                    .GroupBy(x => new { x.Code, x.Name })
                    .Select(g => new FilterGroupDto
                    {
                        Code = g.Key.Code,
                        Name = g.Key.Name,
                        Options = g.Select(opt => new FilterItemDto
                        {
                            Value = opt.Value,
                            Label = opt.Value, // Với attribute, Label thường trùng Value
                            Count = opt.Count
                        }).ToList()
                    })
                    .ToList();

                filterResponse.Attributes = groupedAttributes;
            }

            // ==========================================================
            // KẾT THÚC TÍNH TOÁN BỘ LỌC
            // ==========================================================


            // 4. Phân trang & Lấy dữ liệu sản phẩm (Giữ nguyên)
            // Sắp xếp
            query = query.OrderByDescending(x => x.CreatedAt);

            var products = await query
                .Skip((specParams.PageIndex - 1) * specParams.PageSize)
                .Take(specParams.PageSize)
                .ToListAsync(cancellationToken);

            var dtos = _mapper.Map<List<ProductSummaryDto>>(products);

            var pagedListDto = new PagedList<ProductSummaryDto>(
                dtos,
                totalCount,
                specParams.PageIndex,
                specParams.PageSize
            );

            var response = new ProductListResponse
            {
                Products = pagedListDto,
                Filters = filterResponse // <-- Gán kết quả tính toán vào đây
            };

            return Result<ProductListResponse>.Success(response);
        }
    }
}
