using AutoMapper;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Domain.Models;
using bmak_ecommerce.Domain.Entities.Catalog;
using bmak_ecommerce.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using bmak_ecommerce.Application.Common.Models;

namespace bmak_ecommerce.Application.Features.Products.Commands.CreateProduct
{
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
                Slug = GenerateSlug(request.Name),
                SKU = request.SKU,
                CategoryId = request.CategoryId,

                BasePrice = request.BasePrice,
                SalePrice = request.SalePrice,
                SalesUnit = request.SalesUnit,
                PriceUnit = request.PriceUnit ?? "m²",
                ConversionFactor = request.ConversionFactor > 0 ? request.ConversionFactor : 1,

                SaleStartDate = request.SaleStartDate,
                SaleEndDate = request.SaleEndDate,
                Weight = request.Weight ?? 0,
                IsActive = request.IsActive ?? true,
                Thumbnail = request.ImageUrl,

                // Cấu hình kho
                AllowBackorder = request.AllowBackorder ?? true,
                ManageStock = request.ManageStock ?? true,

                AttributeValues = new List<ProductAttributeValue>(),
                ProductTags = new List<ProductTag>(),
                Stocks = new List<ProductStock>()
            };

            // 3. Xử lý JSON Specifications
            product.SpecificationsJson = BuildSpecificationsJson(request.SpecificationsJson, request.ImageUrl);

            // 4. Xử lý Attributes
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

            // 5. Xử lý Tags
            if (request.TagIds != null && request.TagIds.Any())
            {
                foreach (var tagId in request.TagIds)
                {
                    product.ProductTags.Add(new ProductTag { TagId = tagId });
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

        private string BuildSpecificationsJson(string? jsonInput, string? imageUrl)
        {
            var dict = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(jsonInput))
            {
                try
                {
                    using var doc = JsonDocument.Parse(jsonInput);
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
                catch { }
            }
            if (!string.IsNullOrEmpty(imageUrl)) dict["imageUrl"] = imageUrl;
            return dict.Count > 0 ? JsonSerializer.Serialize(dict) : "{}";
        }
    }
}
