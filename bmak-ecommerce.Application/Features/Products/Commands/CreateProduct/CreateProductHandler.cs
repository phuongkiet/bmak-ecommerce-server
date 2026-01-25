using AutoMapper;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Domain.Entities.Catalog;
using bmak_ecommerce.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Products.Commands.CreateProduct
{
    // Implement ICommandHandler thay vì IRequestHandler
    public class CreateProductHandler : ICommandHandler<CreateProductCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        // Có thể giữ Mapper để map các trường cơ bản cho gọn, hoặc bỏ luôn nếu muốn full manual
        private readonly IMapper _mapper;

        public CreateProductHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            // 1. Khởi tạo Entity thủ công (Manual Mapping) để kiểm soát dữ liệu
            // Cách này an toàn hơn AutoMapper khi logic phức tạp
            var product = new Product
            {
                Name = request.Name,
                SKU = request.SKU,
                Slug = GenerateSlug(request.Name),

                // --- CÁC TRƯỜNG QUAN TRỌNG BẠN CẦN ---
                BasePrice = request.BasePrice,
                SalePrice = request.SalePrice,
                SalesUnit = request.SalesUnit,          // "Viên"
                PriceUnit = request.PriceUnit ?? "m²",  // "m2" (Default nếu null)
                ConversionFactor = request.ConversionFactor > 0 ? request.ConversionFactor : 1, // Default 1

                AllowBackorder = request.AllowBackorder ?? true,
                ManageStock = request.ManageStock ?? true,

                SaleStartDate = request.SaleStartDate,
                SaleEndDate = request.SaleEndDate,
                Weight = request.Weight ?? 0,
                IsActive = request.IsActive ?? true,
                CategoryId = request.CategoryId,

                // Xử lý Thumbnail từ ImageUrl
                Thumbnail = request.ImageUrl,

                // Khởi tạo các list để tránh NullReference
                AttributeValues = new List<ProductAttributeValue>(),
                ProductTags = new List<ProductTag>()
            };

            if (product.ManageStock && request.InitialStock.HasValue && request.InitialStock.Value > 0)
            {
                product.Stocks.Add(new ProductStock
                {
                    // Nếu không nhập tên kho, mặc định là "Kho chính"
                    WarehouseName = !string.IsNullOrEmpty(request.WarehouseName)
                                    ? request.WarehouseName
                                    : "Kho Chính",

                    // Tự sinh số Lô (Batch) cho lần nhập đầu tiên
                    BatchNumber = $"INIT-{DateTime.UtcNow:yyyyMMdd}",

                    QuantityOnHand = request.InitialStock.Value

                    // ProductId sẽ được EF Core tự điền khi SaveChanges
                });
            }

            // 2. Xử lý SpecificationsJson (Logic merge ImageUrl cũ của bạn)
            product.SpecificationsJson = BuildSpecificationsJson(request.SpecificationsJson, request.ImageUrl);

            // 3. Xử lý Attributes (nếu có)
            if (request.Attributes != null && request.Attributes.Any())
            {
                foreach (var attr in request.Attributes)
                {
                    product.AttributeValues.Add(new ProductAttributeValue
                    {
                        AttributeId = attr.AttributeId,
                        Value = attr.Value
                    });
                }
            }

            // 4. Xử lý Tags (nếu có)
            if (request.TagIds != null && request.TagIds.Any())
            {
                // Kiểm tra sơ bộ xem tag có tồn tại không (Optional)
                // var existingTags = await _unitOfWork.Repository<Tag>().GetByIdsAsync(request.TagIds);

                foreach (var tagId in request.TagIds)
                {
                    product.ProductTags.Add(new ProductTag
                    {
                        TagId = tagId
                    });
                }
            }

            // 5. Lưu vào DB
            await _unitOfWork.Repository<Product>().AddAsync(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return product.Id;
        }

        // Tách logic xử lý JSON ra hàm riêng cho code gọn
        private string BuildSpecificationsJson(string? existingJson, string? imageUrl)
        {
            var dict = new Dictionary<string, object>();

            // Parse JSON cũ nếu có
            if (!string.IsNullOrEmpty(existingJson))
            {
                try
                {
                    using var jsonDoc = JsonDocument.Parse(existingJson);
                    foreach (var prop in jsonDoc.RootElement.EnumerateObject())
                    {
                        // Copy dữ liệu cũ
                        switch (prop.Value.ValueKind)
                        {
                            case JsonValueKind.String: dict[prop.Name] = prop.Value.GetString() ?? ""; break;
                            case JsonValueKind.Number: dict[prop.Name] = prop.Value.GetDecimal(); break;
                            case JsonValueKind.True: dict[prop.Name] = true; break;
                            case JsonValueKind.False: dict[prop.Name] = false; break;
                            default: dict[prop.Name] = prop.Value.GetRawText(); break;
                        }
                    }
                }
                catch
                {
                    // Nếu JSON lỗi thì bỏ qua, tạo mới
                }
            }

            // Merge ImageUrl vào JSON (nếu chưa có hoặc muốn override)
            if (!string.IsNullOrEmpty(imageUrl))
            {
                dict["imageUrl"] = imageUrl;
            }

            // Nếu Dictionary rỗng, trả về JSON object rỗng
            return dict.Count > 0 ? JsonSerializer.Serialize(dict) : "{}";
        }

        private string GenerateSlug(string phrase)
        {
            // Nên dùng thư viện "Slugify" để xử lý tiếng Việt: "Gạch men" -> "gach-men"
            // Tạm thời code đơn giản
            if (string.IsNullOrEmpty(phrase)) return string.Empty;
            return phrase.ToLower().Replace(" ", "-"); // Cần cải thiện sau
        }
    }
}
