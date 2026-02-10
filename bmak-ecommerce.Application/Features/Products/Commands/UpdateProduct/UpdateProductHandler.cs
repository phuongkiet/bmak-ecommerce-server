using AutoMapper;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Domain.Models;
using bmak_ecommerce.Domain.Entities.Catalog;
using bmak_ecommerce.Domain.Interfaces;
using MediatR;
using System.Text.Json;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Common.Attributes;

namespace bmak_ecommerce.Application.Features.Products.Commands.UpdateProduct
{
    [AutoRegister]

    public class UpdateProductHandler : ICommandHandler<UpdateProductCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProductHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<int>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            // 1. Lấy sản phẩm từ DB (Phải lấy kèm Attributes & Tags để so sánh)
            var product = await _unitOfWork.Products.GetProductDetailAsync(request.Id);

            if (product == null)
            {
                return Result<int>.Failure($"Không tìm thấy sản phẩm với ID: {request.Id}");
            }

            // 2. Map dữ liệu cơ bản (Manual Mapping)
            product.Name = request.Name;
            product.Slug = GenerateSlug(request.Name); // Update tên thì update luôn Slug
            product.SKU = request.SKU;
            product.CategoryId = request.CategoryId;

            product.BasePrice = request.BasePrice;
            product.SalePrice = request.SalePrice;
            product.SalesUnit = request.SalesUnit;
            product.PriceUnit = request.PriceUnit ?? "m²"; // Mặc định nếu null
            product.ConversionFactor = request.ConversionFactor > 0 ? request.ConversionFactor : 1;

            product.SaleStartDate = request.SaleStartDate;
            product.SaleEndDate = request.SaleEndDate;
            product.Weight = request.Weight ?? 0;
            product.IsActive = request.IsActive ?? true;

            // Chỉ update ảnh nếu request có gửi ảnh mới lên (nếu null/empty thì giữ ảnh cũ)
            if (!string.IsNullOrEmpty(request.ImageUrl))
            {
                product.Thumbnail = request.ImageUrl;
            }

            // 3. Xử lý Specifications JSON (Merge hoặc Ghi đè)
            product.SpecificationsJson = BuildSpecificationsJson(
                request.SpecificationsJson,
                request.ImageUrl,
                product.SpecificationsJson // Truyền JSON cũ vào
            );

            // 4. XỬ LÝ ATTRIBUTES (Khó nhất: Xóa cũ, Thêm mới, Sửa tồn tại)
            UpdateAttributes(product, request.Attributes);

            // 5. XỬ LÝ TAGS
            UpdateTags(product, request.TagIds);

            try
            {
                // 6. Lưu thay đổi
                // Gọi Update để EF Core đánh dấu trạng thái Modified
                _unitOfWork.Repository<Product>().Update(product);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<int>.Success(product.Id);
            }
            catch (Exception ex)
            {
                return Result<int>.Failure($"Lỗi khi cập nhật sản phẩm: {ex.Message}");
            }
        }

        // --- CÁC HÀM HELPER (TÁCH RA CHO GỌN) ---

        private void UpdateAttributes(Product product, List<UpdateProductAttributeDto> requestAttrs)
        {
            if (requestAttrs == null) requestAttrs = new List<UpdateProductAttributeDto>();

            // A. TÌM NHỮNG CÁI CẦN XÓA
            // Logic: Có trong DB nhưng KHÔNG có trong Request -> Xóa
            var requestAttrIds = requestAttrs.Select(x => x.AttributeId).ToList();
            var toRemove = product.AttributeValues
                .Where(x => !requestAttrIds.Contains(x.AttributeId))
                .ToList();

            foreach (var item in toRemove)
            {
                product.AttributeValues.Remove(item);
            }

            // B. TÌM NHỮNG CÁI CẦN THÊM HOẶC SỬA
            foreach (var dto in requestAttrs)
            {
                var existingAttr = product.AttributeValues
                    .FirstOrDefault(x => x.AttributeId == dto.AttributeId);

                if (existingAttr != null)
                {
                    // Case 1: Đã tồn tại -> Update giá trị
                    existingAttr.Value = dto.Value;
                }
                else
                {
                    // Case 2: Chưa có -> Thêm mới
                    product.AttributeValues.Add(new ProductAttributeValue
                    {
                        AttributeId = dto.AttributeId,
                        Value = dto.Value
                        // ProductId tự động được EF gán
                    });
                }
            }
        }

        private void UpdateTags(Product product, List<int> requestTagIds)
        {
            if (requestTagIds == null) requestTagIds = new List<int>();

            // A. XÓA TAGS CŨ
            // Logic: Có trong DB nhưng không có trong Request
            var toRemove = product.ProductTags
                .Where(x => !requestTagIds.Contains(x.TagId))
                .ToList();

            foreach (var item in toRemove)
            {
                product.ProductTags.Remove(item);
            }

            // B. THÊM TAGS MỚI
            // Logic: Có trong Request nhưng chưa có trong DB
            var currentTagIds = product.ProductTags.Select(x => x.TagId).ToList();
            var newTagIds = requestTagIds.Except(currentTagIds);

            foreach (var tagId in newTagIds)
            {
                product.ProductTags.Add(new ProductTag { TagId = tagId });
            }
        }

        private string GenerateSlug(string phrase)
        {
            if (string.IsNullOrEmpty(phrase)) return string.Empty;
            return phrase.ToLower().Trim().Replace(" ", "-");
        }

        private string BuildSpecificationsJson(string? newJson, string? imageUrl, string? oldJson)
        {
            var dict = new Dictionary<string, object>();

            // Ưu tiên dùng JSON mới, nếu không có thì dùng JSON cũ
            string jsonToParse = !string.IsNullOrEmpty(newJson) ? newJson : oldJson;

            if (!string.IsNullOrEmpty(jsonToParse))
            {
                try
                {
                    using var doc = JsonDocument.Parse(jsonToParse);
                    foreach (var prop in doc.RootElement.EnumerateObject())
                    {
                        switch (prop.Value.ValueKind)
                        {
                            case JsonValueKind.String: dict[prop.Name] = prop.Value.GetString(); break;
                            case JsonValueKind.Number: dict[prop.Name] = prop.Value.GetDecimal(); break;
                            case JsonValueKind.True: dict[prop.Name] = true; break;
                            case JsonValueKind.False: dict[prop.Name] = false; break;
                            default: dict[prop.Name] = prop.Value.GetRawText(); break;
                        }
                    }
                }
                catch { /* Ignore JSON error */ }
            }

            // Luôn đảm bảo ImageUrl mới nhất được cập nhật vào JSON
            if (!string.IsNullOrEmpty(imageUrl))
            {
                dict["imageUrl"] = imageUrl;
            }

            return dict.Count > 0 ? JsonSerializer.Serialize(dict) : "{}";
        }
    }
}

