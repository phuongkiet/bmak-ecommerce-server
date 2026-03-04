using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Domain.Models;
using bmak_ecommerce.Domain.Entities.Catalog;
using bmak_ecommerce.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Domain.Entities.Media;
using Microsoft.EntityFrameworkCore;

namespace bmak_ecommerce.Application.Features.Products.Commands.CreateProduct
{
    [AutoRegister]

    public class CreateProductHandler : ICommandHandler<CreateProductCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateProductHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<int>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            // 1. Validate logic nghiệp vụ (Optional)
            // Ví dụ: Check trùng SKU
            // var existingSku = await _unitOfWork.Products.GetBySkuAsync(request.SKU);
            // if (existingSku != null) return Result<int>.Failure($"SKU '{request.SKU}' đã tồn tại.");

            // 2. Map Entity thủ công
            var product = new Product
            {
                Name = request.Name,
                Slug = request.Slug ?? GenerateSlug(request.Name),
                SKU = request.SKU,
                ShortDescription = request.ShortDescription,
                Description = request.Description,
                ProductCategories = new List<ProductCategory>(),

                BasePrice = request.BasePrice,
                SalePrice = request.SalePrice,
                SalesUnit = request.SalesUnit,
                PriceUnit = request.PriceUnit ?? "m²",
                ConversionFactor = request.ConversionFactor > 0 ? request.ConversionFactor : 1,

                Width = request.Width,
                Height = request.Height,
                Thickness = request.Thickness,
                BoxQuantity = request.BoxQuantity,
                Random = request.Random,

                SaleStartDate = request.SaleStartDate,
                SaleEndDate = request.SaleEndDate,
                Weight = request.Weight ?? 0,
                IsActive = request.IsActive ?? true,
                Thumbnail = request.ThumbnailUrl,
                SpecificationsJson = BuildCustomAttributesJson(request.SpecificationsJson),

                // Khởi tạo list rỗng
                Images = new List<AppImage>(),

                // Cấu hình kho
                AllowBackorder = request.AllowBackorder ?? true,
                ManageStock = request.ManageStock ?? true,

                AttributeSelections = new List<ProductAttributeSelection>(),
                ProductTags = new List<ProductTag>(),
                Stocks = new List<ProductStock>()
            };

            if (request.ImageIds != null && request.ImageIds.Any())
            {
                // Cách 1: Query lấy entity từ DB (An toàn nhất)
                // Giả sử Repository có hàm GetList hoặc dùng IUnitOfWork truy cập DbSet trực tiếp
                // Lưu ý: UnitOfWork của bạn cần hỗ trợ query generic

                var imageRepo = _unitOfWork.Repository<AppImage>();

                // Lấy tất cả ảnh có Id nằm trong danh sách gửi lên
                var selectedImages = await imageRepo.GetAllAsQueryable() // Hoặc hàm filter tương đương
                    .Where(img => request.ImageIds.Contains(img.Id))
                    .ToListAsync(cancellationToken);

                if (selectedImages.Any())
                {
                    foreach (var img in selectedImages)
                    {
                        product.Images.Add(img);
                    }

                    // Logic phụ: Nếu chưa có Thumbnail, lấy ảnh đầu tiên làm Thumbnail luôn
                    if (string.IsNullOrEmpty(product.Thumbnail))
                    {
                        product.Thumbnail = selectedImages.First().Url;
                    }
                }
            }

            // 4. Xử lý Attributes (mô hình value dùng chung)
            if (request.Attributes != null && request.Attributes.Any())
            {
                if (request.Attributes.Count > 5)
                {
                    return Result<int>.Failure("Mỗi sản phẩm chỉ được chọn tối đa 5 thuộc tính");
                }

                var duplicateAttributeIds = request.Attributes
                    .GroupBy(x => x.AttributeId)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();

                if (duplicateAttributeIds.Any())
                {
                    return Result<int>.Failure("Mỗi thuộc tính chỉ được chọn một giá trị");
                }

                var requestedValueIds = request.Attributes.Select(x => x.AttributeValueId).Distinct().ToList();
                var values = await _unitOfWork.Repository<ProductAttributeValue>()
                    .GetAllAsQueryable()
                    .Where(x => requestedValueIds.Contains(x.Id))
                    .ToDictionaryAsync(x => x.Id, cancellationToken);

                foreach (var attr in request.Attributes)
                {
                    if (!values.TryGetValue(attr.AttributeValueId, out var attributeValue))
                    {
                        return Result<int>.Failure($"Không tìm thấy AttributeValue với ID: {attr.AttributeValueId}");
                    }

                    if (attributeValue.AttributeId != attr.AttributeId)
                    {
                        return Result<int>.Failure(
                            $"Giá trị thuộc tính ID {attr.AttributeValueId} không thuộc Attribute ID {attr.AttributeId}");
                    }

                    product.AttributeSelections.Add(new ProductAttributeSelection
                    {
                        AttributeId = attr.AttributeId,
                        AttributeValueId = attr.AttributeValueId
                    });
                }
            }

            // 5. Xử lý Tags
            if (request.TagIds != null && request.TagIds.Any())
            {
                foreach (var tagId in request.TagIds)
                {
                    product.ProductTags.Add(new ProductTag { TagId = tagId });
                }
            }

            if (request.CategoryIds != null && request.CategoryIds.Any())
            {
                // Lọc trùng lặp ID nếu FE gửi lỗi
                var distinctCategoryIds = request.CategoryIds.Distinct();
                foreach (var catId in distinctCategoryIds)
                {
                    product.ProductCategories.Add(new ProductCategory { CategoryId = catId });
                }
            }

            // 6. Xử lý Initial Stock
            if (product.ManageStock && request.InitialStock.HasValue && request.InitialStock.Value > 0)
            {
                product.Stocks.Add(new ProductStock
                {
                    WarehouseName = !string.IsNullOrEmpty(request.WarehouseName) ? request.WarehouseName : "Kho Chính",
                    BatchNumber = $"INIT-{DateTime.UtcNow:yyyyMMdd}",
                    QuantityOnHand = request.InitialStock.Value
                });
            }

            try
            {
                await _unitOfWork.Repository<Product>().AddAsync(product);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Trả về Success kèm ID
                return Result<int>.Success(product.Id);
            }
            catch (Exception ex)
            {
                // Log error here
                return Result<int>.Failure($"Lỗi khi tạo sản phẩm: {ex.Message}");
            }
        }

        // --- HELPER METHODS ---
        private string GenerateSlug(string phrase)
        {
            if (string.IsNullOrEmpty(phrase)) return string.Empty;
            return phrase.ToLower().Trim().Replace(" ", "-"); // Nên dùng thư viện Slugify để tốt hơn
        }

        private string BuildCustomAttributesJson(string? jsonInput)
        {
            // Trả về mảng rỗng chuẩn JSON nếu không có dữ liệu
            if (string.IsNullOrWhiteSpace(jsonInput)) return "[]";

            try
            {
                var attributes = new List<object>();
                using var doc = JsonDocument.Parse(jsonInput);

                // Trường hợp 1: Frontend gửi lên dạng Object phẳng { "Bề mặt": "Nhám", "Xuất xứ": "VN" }
                if (doc.RootElement.ValueKind == JsonValueKind.Object)
                {
                    foreach (var prop in doc.RootElement.EnumerateObject())
                    {
                        string val = prop.Value.ValueKind switch
                        {
                            JsonValueKind.String => prop.Value.GetString() ?? string.Empty,
                            JsonValueKind.Number => prop.Value.GetDecimal().ToString(),
                            JsonValueKind.True => "Có",     // Chuẩn hóa boolean sang text
                            JsonValueKind.False => "Không",
                            _ => prop.Value.GetRawText()
                        };

                        if (!string.IsNullOrWhiteSpace(val))
                        {
                            attributes.Add(new { Name = prop.Name.Trim(), Value = val.Trim() });
                        }
                    }
                    return JsonSerializer.Serialize(attributes);
                }

                // Trường hợp 2: Frontend đã gửi lên đúng chuẩn Array [ { "Name": "...", "Value": "..." } ]
                if (doc.RootElement.ValueKind == JsonValueKind.Array)
                {
                    // Trả thẳng về DB luôn, không cần parse lại
                    return jsonInput;
                }

                return "[]";
            }
            catch
            {
                // Tránh sập API nếu Frontend lỡ gửi string tào lao (không phải JSON)
                return "[]";
            }
        }
    }
}
