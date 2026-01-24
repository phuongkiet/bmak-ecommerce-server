using AutoMapper;
using bmak_ecommerce.Domain.Entities.Catalog;
using bmak_ecommerce.Application.Features.Products.Commands.CreateProduct; // Sẽ tạo ở dưới
using System.Text.Json;
using bmak_ecommerce.Application.Features.Products.DTOs.Catalog;

namespace Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // 1. Map Entity -> DTO (Chiều trả về cho khách)
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.CategorySlug, opt => opt.MapFrom(src => src.Category.Slug))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.AttributeValues))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.ProductTags.Select(pt => pt.Tag)))
                .ForMember(dest => dest.Thumbnail, opt => opt.MapFrom(src => ExtractImageUrlFromSpecifications(src.SpecificationsJson)));

            CreateMap<ProductAttributeValue, ProductAttributeDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Attribute.Name))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Attribute.Code));

            // ProductAttribute mapping (cho việc list attributes)
            CreateMap<ProductAttribute, ProductAttributeListItemDto>();

            // Tag mapping
            CreateMap<Tag, TagDto>();

            // 2. Map Command -> Entity (Chiều thêm mới)
            CreateMap<CreateProductCommand, Product>()
                .ForMember(dest => dest.Slug, opt => opt.Ignore()) // Slug sẽ tự sinh
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.SpecificationsJson, opt => opt.Ignore()) // Sẽ set trong Handler (có thể null từ request, ImageUrl sẽ merge vào đây)
                .ForMember(dest => dest.PriceUnit, opt => opt.Ignore()) // Sẽ set trong Handler
                .ForMember(dest => dest.IsActive, opt => opt.Ignore()) // Sẽ set trong Handler (có thể null từ request)
                .ForMember(dest => dest.Weight, opt => opt.Ignore()) // Sẽ set trong Handler (có thể null từ request)
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.AttributeValues, opt => opt.Ignore()) // Sẽ xử lý trong Handler
                .ForMember(dest => dest.ProductTags, opt => opt.Ignore()) // Sẽ xử lý trong Handler
                .ForMember(dest => dest.TierPrices, opt => opt.Ignore())
                .ForMember(dest => dest.Stocks, opt => opt.Ignore());
                // ImageUrl và TagIds không có trong Product Entity, sẽ được xử lý trong Handler

            // ===== CATEGORY MAPPINGS =====
            // 1. Map Entity -> DTO (Chiều trả về cho khách)
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.ParentName, opt => opt.MapFrom(src => src.Parent != null ? src.Parent.Name : null))
                .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products != null ? src.Products.Count : 0));

            // 2. Map Command -> Entity (Chiều thêm mới)
            CreateMap<bmak_ecommerce.Application.Features.Categories.Commands.CreateCategory.CreateCategoryCommand, Category>()
                .ForMember(dest => dest.Slug, opt => opt.Ignore()) // Slug sẽ tự sinh
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Products, opt => opt.Ignore())
                .ForMember(dest => dest.Children, opt => opt.Ignore())
                .ForMember(dest => dest.Parent, opt => opt.Ignore());

            // ===== PRODUCT ATTRIBUTE MAPPINGS =====
            // Map Command -> Entity (Chiều thêm mới)
            CreateMap<bmak_ecommerce.Application.Features.ProductAttributes.Commands.CreateProductAttribute.CreateProductAttributeCommand, ProductAttribute>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Values, opt => opt.Ignore());

            // ===== PRODUCT ATTRIBUTE VALUE MAPPINGS =====
            // Map Command -> Entity (Chiều thêm mới)
            CreateMap<bmak_ecommerce.Application.Features.ProductAttributeValues.Commands.CreateProductAttributeValue.CreateProductAttributeValueCommand, ProductAttributeValue>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.Attribute, opt => opt.Ignore());

            // ===== TAG MAPPINGS =====
            // Map Command -> Entity (Chiều thêm mới)
            CreateMap<bmak_ecommerce.Application.Features.Tags.Commands.CreateTag.CreateTagCommand, Tag>()
                .ForMember(dest => dest.Slug, opt => opt.Ignore()) // Slug sẽ tự sinh
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ProductTags, opt => opt.Ignore());
        }

        // Helper method để extract ImageUrl từ SpecificationsJson
        private static string? ExtractImageUrlFromSpecifications(string? specificationsJson)
        {
            if (string.IsNullOrWhiteSpace(specificationsJson))
            {
                return null;
            }

            try
            {
                var jsonDoc = JsonDocument.Parse(specificationsJson);
                if (jsonDoc.RootElement.TryGetProperty("imageUrl", out var imageUrlElement))
                {
                    return imageUrlElement.GetString();
                }
            }
            catch
            {
                // Nếu không parse được JSON, trả về null
            }

            return null;
        }
    }
}