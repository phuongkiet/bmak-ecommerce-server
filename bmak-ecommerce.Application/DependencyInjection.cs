using Application.Mappings;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Domain.Models;
using bmak_ecommerce.Application.Features.Cart.Commands.AddToCart;
using bmak_ecommerce.Application.Features.Cart.Commands.ClearCart;
using bmak_ecommerce.Application.Features.Cart.Commands.DeleteCartItem;
using bmak_ecommerce.Application.Features.Cart.Commands.UpdateCartItem;
using bmak_ecommerce.Application.Features.Cart.Models;
using bmak_ecommerce.Application.Features.Cart.Queries.GetCart;
using bmak_ecommerce.Application.Features.Orders.Commands.CreateOrder;
using bmak_ecommerce.Application.Features.Orders.Queries.GetAllOrders;
using bmak_ecommerce.Application.Features.Products.Commands.CreateProduct;
using bmak_ecommerce.Application.Features.Products.Commands.UpdateProduct;
using bmak_ecommerce.Application.Features.Products.DTOs.Catalog;
using bmak_ecommerce.Application.Features.Products.DTOs.Sale;
using bmak_ecommerce.Application.Features.Products.Queries.Products.GetAllProducts;
using bmak_ecommerce.Application.Features.Products.Queries.Products.GetProductById;
using bmak_ecommerce.Application.Features.Products.Queries.Products.GetTopSellingProduct;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using bmak_ecommerce.Application.Features.Provinces.Queries;
using bmak_ecommerce.Application.Features.Provinces.Dtos;
using bmak_ecommerce.Application.Features.Wards.Queries;
using bmak_ecommerce.Application.Features.Wards.Dtos;
using bmak_ecommerce.Application.Features.Pages.DTOs;
using bmak_ecommerce.Application.Features.Pages.Queries.GetAllPages;
using bmak_ecommerce.Application.Features.Pages.Queries.GetPageDetail;
using bmak_ecommerce.Application.Features.Pages.Commands.CreatePage;
using bmak_ecommerce.Application.Features.Pages.Commands.UpdatePage;

namespace bmak_ecommerce.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            try
            {
                // 1. Tự động tìm và đăng ký tất cả các AutoMapper Profiles trong Assembly này
                services.AddAutoMapper(typeof(MappingProfile).Assembly);
            }
            catch (System.Reflection.ReflectionTypeLoadException ex)
            {
                var errorMsg = "Error loading AutoMapper profiles:\n" + 
                    string.Join("\n", ex.LoaderExceptions?.Select((e, i) => $"Type {i}: {e?.Message}") ?? Array.Empty<string>());
                throw new InvalidOperationException(errorMsg, ex);
            }

            try
            {
                //// 2. Đăng ký MediatR (Tìm tất cả Handler trong Assembly này)
                services.AddMediatR(cfg => {
                    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                });
            }
            catch (System.Reflection.ReflectionTypeLoadException ex)
            {
                var errorMsg = "Error loading MediatR handlers:\n" + 
                    string.Join("\n", ex.LoaderExceptions?.Select((e, i) => $"Type {i}: {e?.Message}") ?? Array.Empty<string>());
                throw new InvalidOperationException(errorMsg, ex);
            }

            try
            {
                //// 3. Đăng ký FluentValidation (Tìm tất cả Validator)
                services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            }
            catch (System.Reflection.ReflectionTypeLoadException ex)
            {
                var errorMsg = "Error loading FluentValidation validators:\n" + 
                    string.Join("\n", ex.LoaderExceptions?.Select((e, i) => $"Type {i}: {e?.Message}") ?? Array.Empty<string>());
                throw new InvalidOperationException(errorMsg, ex);
            }

            //// 4. Nếu có các Service logic thuần (không dính DB), đăng ký ở đây

            // --- QUERY HANDLERS ---

            services.AddScoped<IQueryHandler<GetOrdersQuery, PagedList<OrderSummaryDto>>, GetOrdersHandler>();

            services.AddScoped<IQueryHandler<GetProductsQuery, ProductListResponse>, GetProductsHandler>();

            services.AddScoped<IQueryHandler<GetTopSellingProductsQuery, List<ProductSummaryDto>>, GetTopSellingHandler>();

            services.AddScoped<IQueryHandler<GetProductByIdQuery, ProductDto?>, GetProductByIdHandler>();

            services.AddScoped<IQueryHandler<GetCartQuery, ShoppingCart>, GetCartHandler>();

            services.AddScoped<IQueryHandler<GetProvinceQuery, PagedList<ProvinceDto>>, GetProvinceHandler>();

            services.AddScoped<IQueryHandler<GetWardQuery, PagedList<WardDto>>, GetWardHandler>();

            services.AddScoped<IQueryHandler<GetPageQuery, PagedList<PageSummaryDto>>, GetPageHandler>();

            services.AddScoped<IQueryHandler<GetPageDetailQuery, PageDto>, GetPageDetailHandler>();

            // --- COMMAND HANDLERS ---

            services.AddScoped<ICommandHandler<CreateProductCommand, int>, CreateProductHandler>();

            services.AddScoped<ICommandHandler<UpdateProductCommand, int>, UpdateProductHandler>();

            services.AddScoped<ICommandHandler<CreateOrderCommand, int>, CreateOrderHandler>();

            services.AddScoped<ICommandHandler<AddToCartCommand, ShoppingCart>, AddToCartHandler>();

            services.AddScoped<ICommandHandler<UpdateCartItemCommand, ShoppingCart>, UpdateCartItemHandler>();

            services.AddScoped<ICommandHandler<DeleteCartItemCommand, ShoppingCart>, DeleteCartItemHandler>();

            services.AddScoped<ICommandHandler<ClearCartCommand, ShoppingCart>, ClearCartHandler>();

            services.AddScoped<ICommandHandler<CreatePageCommand, string>, CreatePageHandler>();

            services.AddScoped<ICommandHandler<UpdatePageCommand, string>, UpdatePageHandler>();

            return services;
        }
    }
}
