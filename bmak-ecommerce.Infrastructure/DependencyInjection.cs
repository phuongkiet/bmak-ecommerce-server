using bmak_ecommerce.Domain.Entities.Identity;
using bmak_ecommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using bmak_ecommerce.Domain.Interfaces;
using bmak_ecommerce.Infrastructure.Repositories;
using StackExchange.Redis;
using bmak_ecommerce.Infrastructure.MessageBus.Consumers;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Infrastructure.MessageBus;
using MassTransit; // Bắt buộc

namespace bmak_ecommerce.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. Cấu hình Database (MySQL)
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var redisConnectionString = configuration.GetConnectionString("Redis") ?? "localhost:6379";

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });

            // 2. Cấu hình Identity
            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            // 3. Đăng ký Repositories & UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();

            // =================================================================
            // 4. CẤU HÌNH MASSTRANSIT (ĐÂY LÀ CHỖ QUAN TRỌNG ĐỂ FIX LỖI)
            // =================================================================
            services.AddMassTransit(x =>
            {
                // Đăng ký các Consumer (Người nhận tin nhắn)
                x.AddConsumer<OrderCreatedConsumer>();

                // Cấu hình kết nối RabbitMQ
                x.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitHost = configuration["RabbitMQ:Host"] ?? "localhost";
                    var rabbitUser = configuration["RabbitMQ:User"] ?? "guest";
                    var rabbitPass = configuration["RabbitMQ:Pass"] ?? "guest";

                    cfg.Host(rabbitHost, "/", h =>
                    {
                        h.Username(rabbitUser);
                        h.Password(rabbitPass);
                    });

                    // Dòng này cực quan trọng: Nó giúp MassTransit tự động tạo Queue cho Consumer
                    cfg.ConfigureEndpoints(context);
                });
            });

            // 5. Đăng ký Adapter (Cầu nối giữa Clean Arch và MassTransit)
            // Adapter này cần IPublishEndpoint (được tạo ra ở bước 4 phía trên)
            services.AddScoped<IMessageBus, MassTransitBusAdapter>();

            // 6. Cấu hình Redis (Giữ nguyên)
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var config = ConfigurationOptions.Parse(redisConnectionString, true);
                config.AbortOnConnectFail = false;
                return ConnectionMultiplexer.Connect(config);
            });

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = "BmakShop_";
            });

            return services;
        }
    }
}