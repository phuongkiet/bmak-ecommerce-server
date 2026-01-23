using AutoMapper;
using bmak_ecommerce.Domain.Entities.Catalog;
using bmak_ecommerce.Domain.Interfaces;
using MediatR;

namespace bmak_ecommerce.Application.Features.Products.Commands.UpdateProduct
{
    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;

        public UpdateProductHandler(IUnitOfWork unitOfWork, IMapper mapper, IProductRepository productRepository)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _productRepository = productRepository;
        }

        public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            // 1. Lấy product hiện tại từ DB với navigation properties (dùng IProductRepository)
            var product = await _productRepository.GetProductDetailAsync(request.Id);
            
            if (product == null)
            {
                throw new InvalidOperationException($"Không tìm thấy Product với ID: {request.Id}");
            }

            // 2. Validate CategoryId tồn tại
            var category = await _unitOfWork.Repository<Category>().GetByIdAsync(request.CategoryId);
            if (category == null)
            {
                throw new InvalidOperationException($"Không tìm thấy Category với ID: {request.CategoryId}");
            }

            // 3. Validate SKU unique (nếu SKU thay đổi)
            if (product.SKU != request.SKU)
            {
                var existingProducts = await _unitOfWork.Repository<Product>()
                    .FindAsync(p => p.SKU == request.SKU);
                
                if (existingProducts.Any(p => p.Id != request.Id))
                {
                    throw new InvalidOperationException($"SKU '{request.SKU}' đã tồn tại cho sản phẩm khác");
                }
            }

            // 4. Update các fields cơ bản
            product.Name = request.Name;
            product.SKU = request.SKU;
            product.BasePrice = request.BasePrice;
            product.SalePrice = request.SalePrice;
            product.SalesUnit = request.SalesUnit;
            product.ConversionFactor = request.ConversionFactor;
            product.CategoryId = request.CategoryId;
            product.SaleStartDate = request.SaleStartDate;
            product.SaleEndDate = request.SaleEndDate;

            // Update Slug nếu Name thay đổi
            if (product.Name != request.Name)
            {
                product.Slug = GenerateSlug(request.Name);
            }

            // Update PriceUnit
            product.PriceUnit = string.IsNullOrEmpty(request.PriceUnit) ? "m²" : request.PriceUnit;

            // Update Weight
            product.Weight = request.Weight ?? 0;

            // Update IsActive
            product.IsActive = request.IsActive ?? true;

            // Update SpecificationsJson (merge ImageUrl nếu có)
            if (string.IsNullOrEmpty(request.SpecificationsJson))
            {
                if (!string.IsNullOrEmpty(request.ImageUrl))
                {
                    product.SpecificationsJson = $"{{\"imageUrl\":\"{request.ImageUrl}\"}}";
                }
                else if (string.IsNullOrEmpty(product.SpecificationsJson))
                {
                    product.SpecificationsJson = "{}";
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(request.ImageUrl))
                {
                    try
                    {
                        using var jsonDoc = System.Text.Json.JsonDocument.Parse(request.SpecificationsJson);
                        var jsonObject = jsonDoc.RootElement;
                        
                        if (!jsonObject.TryGetProperty("imageUrl", out _))
                        {
                            var dict = new Dictionary<string, object>();
                            foreach (var prop in jsonObject.EnumerateObject())
                            {
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
                            var dict = new Dictionary<string, object>();
                            foreach (var prop in jsonObject.EnumerateObject())
                            {
                                if (prop.Name == "imageUrl") continue;
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
                    }
                    catch
                    {
                        product.SpecificationsJson = $"{{\"imageUrl\":\"{request.ImageUrl}\"}}";
                    }
                }
                else
                {
                    product.SpecificationsJson = request.SpecificationsJson;
                }
            }

            // 5. Update Attributes (xóa cũ và tạo mới)
            if (request.Attributes != null)
            {
                // Xóa tất cả attributes hiện tại (đã được load trong GetProductDetailAsync)
                var existingAttributes = product.AttributeValues.ToList();
                foreach (var attr in existingAttributes)
                {
                    _unitOfWork.Repository<ProductAttributeValue>().Remove(attr);
                }
                product.AttributeValues.Clear();

                // Thêm attributes mới
                foreach (var attr in request.Attributes)
                {
                    // Validate AttributeId tồn tại
                    var attribute = await _unitOfWork.Repository<ProductAttribute>().GetByIdAsync(attr.AttributeId);
                    if (attribute == null)
                    {
                        throw new InvalidOperationException($"Không tìm thấy ProductAttribute với ID: {attr.AttributeId}");
                    }

                    product.AttributeValues.Add(new ProductAttributeValue
                    {
                        AttributeId = attr.AttributeId,
                        Value = attr.Value,
                        ProductId = request.Id
                    });
                }
            }

            // 6. Update Tags (xóa cũ và tạo mới)
            if (request.TagIds != null)
            {
                // Xóa tất cả tags hiện tại (đã được load trong GetProductDetailAsync)
                var existingTags = product.ProductTags.ToList();
                foreach (var tag in existingTags)
                {
                    _unitOfWork.Repository<ProductTag>().Remove(tag);
                }
                product.ProductTags.Clear();

                // Thêm tags mới
                foreach (var tagId in request.TagIds)
                {
                    var tag = await _unitOfWork.Repository<Tag>().GetByIdAsync(tagId);
                    if (tag == null)
                    {
                        throw new InvalidOperationException($"Không tìm thấy Tag với ID: {tagId}");
                    }

                    product.ProductTags.Add(new ProductTag
                    {
                        TagId = tagId,
                        ProductId = request.Id
                    });
                }
            }

            // 7. Update entity
            _unitOfWork.Repository<Product>().Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

        private string GenerateSlug(string phrase)
        {
            return phrase.ToLower()
                .Replace(" ", "-")
                .Replace("đ", "d")
                .Replace("Đ", "d");
        }
    }
}

