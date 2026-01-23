using bmak_ecommerce.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Infrastructure.Persistence
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // Đảm bảo database đã được tạo
            await context.Database.EnsureCreatedAsync();

            // Seed Categories
            if (!await context.Categories.AnyAsync())
            {
                await SeedCategoriesAsync(context);
            }

            // Seed ProductAttributes
            if (!await context.ProductAttributes.AnyAsync())
            {
                await SeedProductAttributesAsync(context);
            }

            // Seed Tags
            if (!await context.Tags.AnyAsync())
            {
                await SeedTagsAsync(context);
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedCategoriesAsync(AppDbContext context)
        {
            var categories = new List<Category>
            {
                new Category
                {
                    Name = "Vật liệu xây dựng",
                    Slug = "vat-lieu-xay-dung",
                    Description = "Các loại vật liệu xây dựng cơ bản",
                    ParentId = null,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                },
                new Category
                {
                    Name = "Gạch ốp tường",
                    Slug = "gach-op-tuong",
                    Description = "Gạch ốp tường các loại",
                    ParentId = null, // Sẽ được set sau khi có ID của "Vật liệu xây dựng"
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                },
                new Category
                {
                    Name = "Gạch lát nền",
                    Slug = "gach-lat-nen",
                    Description = "Gạch lát nền các loại",
                    ParentId = null,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                },
                new Category
                {
                    Name = "Xi măng",
                    Slug = "xi-mang",
                    Description = "Các loại xi măng",
                    ParentId = null,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                },
                new Category
                {
                    Name = "Sắt thép",
                    Slug = "sat-thep",
                    Description = "Sắt thép xây dựng",
                    ParentId = null,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                }
            };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();

            // Set ParentId cho các danh mục con (nếu cần)
            var vatLieuXayDung = await context.Categories.FirstOrDefaultAsync(c => c.Slug == "vat-lieu-xay-dung");
            if (vatLieuXayDung != null)
            {
                var gachOpTuong = await context.Categories.FirstOrDefaultAsync(c => c.Slug == "gach-op-tuong");
                var gachLatNen = await context.Categories.FirstOrDefaultAsync(c => c.Slug == "gach-lat-nen");
                
                if (gachOpTuong != null)
                {
                    gachOpTuong.ParentId = vatLieuXayDung.Id;
                }
                
                if (gachLatNen != null)
                {
                    gachLatNen.ParentId = vatLieuXayDung.Id;
                }

                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedProductAttributesAsync(AppDbContext context)
        {
            var attributes = new List<ProductAttribute>
            {
                new ProductAttribute
                {
                    Name = "Kích thước",
                    Code = "SIZE",
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                },
                new ProductAttribute
                {
                    Name = "Màu sắc",
                    Code = "COLOR",
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                },
                new ProductAttribute
                {
                    Name = "Bề mặt",
                    Code = "SURFACE",
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                },
                new ProductAttribute
                {
                    Name = "Chất liệu",
                    Code = "MATERIAL",
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                },
                new ProductAttribute
                {
                    Name = "Độ dày",
                    Code = "THICKNESS",
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                },
                new ProductAttribute
                {
                    Name = "Xuất xứ",
                    Code = "ORIGIN",
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                }
            };

            await context.ProductAttributes.AddRangeAsync(attributes);
            await context.SaveChangesAsync();
        }

        private static async Task SeedTagsAsync(AppDbContext context)
        {
            var tags = new List<Tag>
            {
                new Tag
                {
                    Name = "Bán chạy",
                    Slug = "ban-chay",
                    Description = "Sản phẩm bán chạy",
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                },
                new Tag
                {
                    Name = "Mới",
                    Slug = "moi",
                    Description = "Sản phẩm mới",
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                },
                new Tag
                {
                    Name = "Khuyến mãi",
                    Slug = "khuyen-mai",
                    Description = "Sản phẩm đang khuyến mãi",
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                },
                new Tag
                {
                    Name = "Cao cấp",
                    Slug = "cao-cap",
                    Description = "Sản phẩm cao cấp",
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                },
                new Tag
                {
                    Name = "Giá tốt",
                    Slug = "gia-tot",
                    Description = "Sản phẩm giá tốt",
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                }
            };

            await context.Tags.AddRangeAsync(tags);
            await context.SaveChangesAsync();
        }
    }
}

