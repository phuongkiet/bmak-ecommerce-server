using Application.Mappings;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Orders.Commands.CreateOrder;
using bmak_ecommerce.Application.Features.Products.Commands.CreateProduct;
using bmak_ecommerce.Application.Features.Products.DTOs.Catalog;
using bmak_ecommerce.Application.Features.Products.DTOs.Sale;
using bmak_ecommerce.Application.Features.Products.Queries.Orders.GetAllOrders;
using bmak_ecommerce.Application.Features.Products.Queries.Products.GetAllProducts;
using bmak_ecommerce.Application.Features.Products.Queries.Products.GetProductById;
using bmak_ecommerce.Application.Features.Products.Queries.Products.GetTopSellingProduct;
using bmak_ecommerce.Domain.Models;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

            //Query DI
            services.AddScoped<IQueryHandler<GetOrdersQuery, Result<PagedList<OrderSummaryDto>>>,GetOrdersHandler>();
            services.AddScoped<IQueryHandler<GetProductsQuery, ProductListResponse>, GetProductsHandler>();
            services.AddScoped<IQueryHandler<GetTopSellingProductsQuery, List<ProductSummaryDto>>,GetTopSellingProductsHandler>();
            services.AddScoped<IQueryHandler<GetProductByIdQuery, ProductDto?>, GetProductByIdHandler>();

            //Command DI
            services.AddScoped<ICommandHandler<CreateProductCommand, int>, CreateProductHandler>();
            services.AddScoped<ICommandHandler<CreateOrderCommand, Result<int>>, CreateOrderCommandHandler>();

            return services;
        }
    }
}
