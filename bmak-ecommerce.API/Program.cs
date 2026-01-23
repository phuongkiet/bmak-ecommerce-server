using bmak_ecommerce.API.Extensions;
using bmak_ecommerce.Application;
using bmak_ecommerce.Infrastructure;
using bmak_ecommerce.Infrastructure.Persistence;

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


// --- 2. Build App ---
var app = builder.Build();


// 1. Development Environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Auto Migration & Seed Data khi chạy Dev
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        // await dbContext.Database.MigrateAsync(); // Tự động update DB nếu cần
        
        // Seed dữ liệu ban đầu
        await DbSeeder.SeedAsync(dbContext);
    }
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