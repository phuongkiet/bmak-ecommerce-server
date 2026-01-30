using bmak_ecommerce.API.Extensions;
using bmak_ecommerce.Application;
using bmak_ecommerce.Domain.Entities.Identity;
using bmak_ecommerce.Infrastructure;
using bmak_ecommerce.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Đăng ký Services (Dependency Injection) ---

// Layer API (Controllers, Swagger...)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Layer Application (Logic)
builder.Services.AddApplicationServices();

// Layer Infrastructure (Database, Identity)
builder.Services.AddInfrastructureServices(builder.Configuration);

//builder.Services.AddStackExchangeRedisCache(options =>
//{
//    // Kết nối đến container docker đang chạy
//    options.Configuration = "localhost:6379";
//    options.InstanceName = "BmakShop_"; // Prefix cho key tránh trùng lặp
//});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    // Fix lỗi trùng tên Schema khi dùng Generic (Result<T>, PagedList<T>)
    options.CustomSchemaIds(type => type.ToString());
});

// --- 2. Build App ---
var app = builder.Build();


// 1. Development Environment
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<AppDbContext>();

        // --- LẤY THÊM 2 SERVICES NÀY ---
        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
        // -------------------------------

        var logger = services.GetRequiredService<ILogger<Program>>();

        try
        {
            await context.Database.MigrateAsync();

            // Truyền đủ 3 tham số vào
            await DbSeeder.SeedAsync(context, userManager, roleManager);
            await DbSeeder.SeedProvincesAsync(context);
            await DbSeeder.SeedWardsAsync(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Lỗi khi Seed dữ liệu");
        }
    }

    app.UseSwagger();
    app.UseSwaggerUI();
}

// 2. Global Middleware
app.UseHttpsRedirection();

// CORS (Nếu frontend React chạy port khác)
app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

// 3. Auth
app.UseAuthentication();
app.UseAuthorization();

// 4. Map Controllers
app.MapControllers();


// --- 4. Run ---
app.Run();