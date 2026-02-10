using bmak_ecommerce.Application.Common.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Infrastructure.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddAutoRegisteredServices(this IServiceCollection services, Assembly assembly)
        {
            // 1. Tìm tất cả các class trong assembly có gắn [AutoRegister]
            var classesToRegister = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.GetCustomAttribute<AutoRegisterAttribute>() != null);

            foreach (var type in classesToRegister)
            {
                var attribute = type.GetCustomAttribute<AutoRegisterAttribute>();

                // 2. Tìm các Interface mà class này implement
                // (Bỏ qua IDisposable hoặc các system interface nếu cần kỹ hơn)
                var interfaces = type.GetInterfaces();

                if (interfaces.Any())
                {
                    foreach (var i in interfaces)
                    {
                        // 3. Đăng ký vào DI Container
                        // Ví dụ: services.AddScoped<IImageService, CloudinaryService>();
                        // Hoặc: services.AddScoped<ICommandHandler<...>, UploadHandler>();

                        var descriptor = new ServiceDescriptor(i, type, attribute.Lifetime);
                        services.Add(descriptor);
                    }
                }
                else
                {
                    // Trường hợp class không có interface (đăng ký chính nó)
                    var descriptor = new ServiceDescriptor(type, type, attribute.Lifetime);
                    services.Add(descriptor);
                }
            }

            return services;
        }
    }
}
