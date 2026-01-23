using bmak_ecommerce.Infrastructure.Persistence;

namespace bmak_ecommerce.API.Extensions
{
    public static class WebApplicationExtensions
    {
        public static async Task<WebApplication> ConfigurePipelineAsync(this WebApplication app)
        {
            // 1. Development Environment
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();

                // (Tùy chọn) Auto Migration & Seed Data khi chạy Dev
                using (var scope = app.Services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    // await dbContext.Database.MigrateAsync(); // Tự động update DB
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

            return app;
        }
    }
}
