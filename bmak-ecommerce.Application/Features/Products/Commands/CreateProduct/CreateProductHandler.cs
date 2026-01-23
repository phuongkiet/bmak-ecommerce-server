using AutoMapper;
using bmak_ecommerce.Domain.Entities.Catalog;
using bmak_ecommerce.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductHandler : IRequestHandler<CreateProductCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateProductHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            // 1. Map Command -> Entity
            var product = _mapper.Map<Product>(request);

            // 2. Logic nghiệp vụ bổ sung
            // Tự động sinh Slug từ Name (Ví dụ: "Gạch 60x60" -> "gach-60x60")
            product.Slug = GenerateSlug(product.Name);

            // Set giá trị mặc định cho các field optional
            // SpecificationsJson: merge ImageUrl vào nếu có
            if (string.IsNullOrEmpty(request.SpecificationsJson))
            {
                // Nếu có ImageUrl, tạo JSON với ImageUrl
                if (!string.IsNullOrEmpty(request.ImageUrl))
                {
                    product.SpecificationsJson = $"{{\"imageUrl\":\"{request.ImageUrl}\"}}";
                }
                else
                {
                    product.SpecificationsJson = "{}"; // JSON rỗng mặc định
                }
            }
            else
            {
                // Nếu đã có SpecificationsJson từ request, merge ImageUrl vào nếu có
                if (!string.IsNullOrEmpty(request.ImageUrl))
                {
                    try
                    {
                        using var jsonDoc = System.Text.Json.JsonDocument.Parse(request.SpecificationsJson);
                        var jsonObject = jsonDoc.RootElement;
                        
                        // Check xem đã có imageUrl chưa, nếu chưa thì merge vào
                        if (!jsonObject.TryGetProperty("imageUrl", out _))
                        {
                            var dict = new Dictionary<string, object>();
                            foreach (var prop in jsonObject.EnumerateObject())
                            {
                                // Convert JSON element to object properly
                                if (prop.Value.ValueKind == System.Text.Json.JsonValueKind.String)
                                    dict[prop.Name] = prop.Value.GetString() ?? string.Empty;
                                else if (prop.Value.ValueKind == System.Text.Json.JsonValueKind.Number)
                                    dict[prop.Name] = prop.Value.GetDecimal();
                                else if (prop.Value.ValueKind == System.Text.Json.JsonValueKind.True || prop.Value.ValueKind == System.Text.Json.JsonValueKind.False)
                                    dict[prop.Name] = prop.Value.GetBoolean();
                                else
                                    dict[prop.Name] = prop.Value.GetRawText();
                            }
                            dict["imageUrl"] = request.ImageUrl;
                            product.SpecificationsJson = System.Text.Json.JsonSerializer.Serialize(dict);
                        }
                        else
                        {
                            // Đã có imageUrl, ưu tiên ImageUrl từ request (override)
                            var dict = new Dictionary<string, object>();
                            foreach (var prop in jsonObject.EnumerateObject())
                            {
                                if (prop.Name == "imageUrl") continue; // Skip existing imageUrl
                                if (prop.Value.ValueKind == System.Text.Json.JsonValueKind.String)
                                    dict[prop.Name] = prop.Value.GetString() ?? string.Empty;
                                else if (prop.Value.ValueKind == System.Text.Json.JsonValueKind.Number)
                                    dict[prop.Name] = prop.Value.GetDecimal();
                                else if (prop.Value.ValueKind == System.Text.Json.JsonValueKind.True || prop.Value.ValueKind == System.Text.Json.JsonValueKind.False)
                                    dict[prop.Name] = prop.Value.GetBoolean();
                                else
                                    dict[prop.Name] = prop.Value.GetRawText();
                            }
                            dict["imageUrl"] = request.ImageUrl; // Override với ImageUrl từ request
                            product.SpecificationsJson = System.Text.Json.JsonSerializer.Serialize(dict);
                        }
                    }
                    catch
                    {
                        // Nếu parse lỗi, tạo mới JSON với ImageUrl
                        product.SpecificationsJson = $"{{\"imageUrl\":\"{request.ImageUrl}\"}}";
                    }
                }
                else
                {
                    // Không có ImageUrl, giữ nguyên SpecificationsJson
                    product.SpecificationsJson = request.SpecificationsJson;
                }
            }

            // PriceUnit: nếu null hoặc empty thì set "m²"
            if (string.IsNullOrEmpty(product.PriceUnit))
            {
                product.PriceUnit = "m²"; // Đơn vị giá mặc định
            }

            // Weight: nếu null thì set 0
            if (!request.Weight.HasValue)
            {
                product.Weight = 0; // Mặc định trọng lượng = 0
            }
            else
            {
                product.Weight = request.Weight.Value;
            }

            // IsActive: nếu null thì set true
            if (!request.IsActive.HasValue)
            {
                product.IsActive = true; // Mặc định active
            }
            else
            {
                product.IsActive = request.IsActive.Value;
            }

            // Xử lý Attributes (nếu có)
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

            // Xử lý Tags (nếu có)
            if (request.TagIds != null && request.TagIds.Any())
            {
                foreach (var tagId in request.TagIds)
                {
                    // Validate Tag tồn tại
                    var tag = await _unitOfWork.Repository<Tag>().GetByIdAsync(tagId);
                    if (tag == null)
                    {
                        throw new InvalidOperationException($"Không tìm thấy Tag với ID: {tagId}");
                    }

                    product.ProductTags.Add(new ProductTag
                    {
                        TagId = tagId
                    });
                }
            }

            // 3. Lưu vào DB thông qua UnitOfWork
            await _unitOfWork.Repository<Product>().AddAsync(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 4. Trả về ID
            return product.Id;
        }

        // Helper sinh slug đơn giản (Nên tách ra Helper class riêng)
        private string GenerateSlug(string phrase)
        {
            // Logic đơn giản: chuyển thường, thay khoảng trắng bằng gạch ngang
            // Thực tế nên dùng thư viện "Slugify" để xử lý tiếng Việt có dấu
            return phrase.ToLower().Replace(" ", "-");
        }
    }
}
