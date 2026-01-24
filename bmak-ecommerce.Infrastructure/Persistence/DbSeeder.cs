using bmak_ecommerce.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bmak_ecommerce.Infrastructure.Persistence
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // 1. Kiểm tra nếu đã có Product thì thôi, không seed nữa để tránh duplicate
            if (await context.Products.AnyAsync()) return;

            // ---------------------------------------------------------
            // SEED ATTRIBUTES (Màu, Size, RAM...)
            // ---------------------------------------------------------
            var colorAttr = new ProductAttribute { Code = "COLOR", Name = "Màu sắc" };
            var sizeAttr = new ProductAttribute { Code = "SIZE", Name = "Kích thước" };
            var ramAttr = new ProductAttribute { Code = "RAM", Name = "Dung lượng RAM" };

            // Kiểm tra từng attribute, nếu chưa có thì thêm vào DB
            // Logic này an toàn hơn là AddRangeAsync mù quáng
            if (!await context.ProductAttributes.AnyAsync(a => a.Code == "COLOR"))
                await context.ProductAttributes.AddAsync(colorAttr);
            else
                colorAttr = await context.ProductAttributes.FirstAsync(a => a.Code == "COLOR");

            if (!await context.ProductAttributes.AnyAsync(a => a.Code == "SIZE"))
                await context.ProductAttributes.AddAsync(sizeAttr);
            else
                sizeAttr = await context.ProductAttributes.FirstAsync(a => a.Code == "SIZE");

            if (!await context.ProductAttributes.AnyAsync(a => a.Code == "RAM"))
                await context.ProductAttributes.AddAsync(ramAttr);
            else
                ramAttr = await context.ProductAttributes.FirstAsync(a => a.Code == "RAM");

            await context.SaveChangesAsync(); // Lưu Attributes trước

            // ---------------------------------------------------------
            // SEED CATEGORIES (Điện thoại, Thời trang)
            // ---------------------------------------------------------
            var catPhone = new Category { Name = "Điện thoại", Slug = "dien-thoai", IsDeleted = false, Description = "Điện thoại di động"};
            var catFashion = new Category { Name = "Thời trang", Slug = "thoi-trang", IsDeleted = false, Description = "Thời trang gia dụng" };

            if (!await context.Categories.AnyAsync(c => c.Slug == "dien-thoai"))
                await context.Categories.AddAsync(catPhone);
            else
                catPhone = await context.Categories.FirstAsync(c => c.Slug == "dien-thoai");

            if (!await context.Categories.AnyAsync(c => c.Slug == "thoi-trang"))
                await context.Categories.AddAsync(catFashion);
            else
                catFashion = await context.Categories.FirstAsync(c => c.Slug == "thoi-trang");

            await context.SaveChangesAsync(); // Lưu Categories

            // ---------------------------------------------------------
            // SEED PRODUCTS (Sản phẩm mẫu)
            // ---------------------------------------------------------
            var products = new List<Product>();

            // --- SP 1: iPhone 15 ---
            var p1 = new Product
            {
                Name = "iPhone 15 Pro Max 256GB",
                Slug = "iphone-15-pro-max",
                SKU = "IP15PM-256",
                BasePrice = 34990000,
                SalePrice = 32990000,
                Stocks = new List<ProductStock>
                {
                    new ProductStock
                    {
                        // Không cần gán Id = 1 (DB tự sinh)
                        // Không cần gán ProductId = 1 (EF Core tự link với p1)
                        BatchNumber = "BATCH-001",
                        WarehouseName = "952",
                        QuantityOnHand = 10,
                        CreatedAt = DateTime.Now,
                        IsDeleted = false,
                        UpdatedAt = DateTime.Now,
                    }
                },
                Category = catPhone,
                Thumbnail = "https://cdn.tgdd.vn/Products/Images/42/305658/iphone-15-pro-max-blue-thumbnew-600x600.jpg",
                IsActive = true,
                IsDeleted = false,
                SpecificationsJson = "{\"Màn hình\": \"6.7 inch, Super Retina XDR\", \"Chip\": \"Apple A17 Pro\", \"RAM\": \"8 GB\", \"Dung lượng\": \"256 GB\", \"Camera sau\": \"48MP + 12MP + 12MP\", \"Pin\": \"4422 mAh\"}",
                PriceUnit = "viên",
                SalesUnit = "m2"
            };
            // Ảnh chi tiết
            p1.ProductImages.Add(new ProductImage { ImageUrl = p1.Thumbnail, IsMain = true, SortOrder = 1 });
            p1.ProductImages.Add(new ProductImage { ImageUrl = "https://cdn.tgdd.vn/Products/Images/42/305658/iphone-15-pro-max-titan-1.jpg", IsMain = false, SortOrder = 2 });

            // Thuộc tính (FIX: Dùng 'Attributes' và 'ProductAttribute')
            p1.AttributeValues.Add(new ProductAttributeValue { Attribute = colorAttr, Value = "Titan Tự Nhiên" });
            p1.AttributeValues.Add(new ProductAttributeValue { Attribute = ramAttr, Value = "8GB" });

            // Kho hàng (Optional: Nếu bạn tách bảng Stock riêng)
            p1.Stocks.Add(new ProductStock { WarehouseName = "Kho Tổng", QuantityOnHand = 50, BatchNumber = "BATCH-001" });

            products.Add(p1);

            // --- SP 2: Samsung S24 ---
            var p2 = new Product
            {
                Name = "Samsung Galaxy S24 Ultra",
                Slug = "samsung-s24-ultra",
                SKU = "SS-S24U",
                BasePrice = 31990000,
                SalePrice = 29990000,
                Stocks = new List<ProductStock>
                {
                    new ProductStock
                    {
                        // Không cần gán Id = 1 (DB tự sinh)
                        // Không cần gán ProductId = 1 (EF Core tự link với p1)
                        BatchNumber = "BATCH-001",
                        WarehouseName = "952",
                        QuantityOnHand = 10,
                        CreatedAt = DateTime.Now,
                        IsDeleted = false,
                        UpdatedAt = DateTime.Now,
                    }
                },
                Category = catPhone,
                Thumbnail = "https://cdn.tgdd.vn/Products/Images/42/307174/samsung-galaxy-s24-ultra-grey-thumbnew-600x600.jpg",
                IsActive = true,
                IsDeleted = false,
                SpecificationsJson = "{\"Màn hình\": \"6.8 inch, Dynamic AMOLED 2X\", \"Chip\": \"Snapdragon 8 Gen 3\", \"RAM\": \"12 GB\", \"Bộ nhớ\": \"512 GB\", \"Pin\": \"5000 mAh\", \"Sạc nhanh\": \"45W\"}",
                PriceUnit = "viên",
                SalesUnit = "m2"
            };
            p2.ProductImages.Add(new ProductImage { ImageUrl = p2.Thumbnail, IsMain = true, SortOrder = 1 });
            p2.AttributeValues.Add(new ProductAttributeValue { Attribute = colorAttr, Value = "Đen Titan" });
            p2.AttributeValues.Add(new ProductAttributeValue { Attribute = ramAttr, Value = "12GB" });
            p2.Stocks.Add(new ProductStock { WarehouseName = "Kho Tổng", QuantityOnHand = 20, BatchNumber = "BATCH-002" });

            products.Add(p2);

            // --- SP 3: Áo Thun Đen ---
            var p3 = new Product
            {
                Name = "Áo Thun Basic Coolmate",
                Slug = "ao-thun-basic",
                SKU = "TSHIRT-BLK-L",
                BasePrice = 150000,
                SalePrice = 129000,
                Stocks = new List<ProductStock>
                {
                    new ProductStock
                    {
                        // Không cần gán Id = 1 (DB tự sinh)
                        // Không cần gán ProductId = 1 (EF Core tự link với p1)
                        BatchNumber = "BATCH-001",
                        WarehouseName = "952",
                        QuantityOnHand = 10,
                        CreatedAt = DateTime.Now,
                        IsDeleted = false,
                        UpdatedAt = DateTime.Now,
                    }
                },
                Category = catFashion,
                Thumbnail = "https://m.yodycdn.com/fit-in/filters:format(webp)/products/ao-thun-nam-tsm5193-den-4.jpg",
                IsActive = true,
                IsDeleted = false,
                SpecificationsJson = "{\"Chất liệu\": \"100% Cotton\", \"Kiểu dáng\": \"Regular Fit\", \"Cổ áo\": \"Cổ tròn\", \"Xuất xứ\": \"Việt Nam\", \"Hướng dẫn\": \"Giặt máy ở nhiệt độ thường\"}",
                PriceUnit = "viên",
                SalesUnit = "m2"
            };
            p3.ProductImages.Add(new ProductImage { ImageUrl = p3.Thumbnail, IsMain = true, SortOrder = 1 });
            p3.AttributeValues.Add(new ProductAttributeValue { Attribute = colorAttr, Value = "Đen" });
            p3.AttributeValues.Add(new ProductAttributeValue { Attribute = sizeAttr, Value = "L" });
            p3.Stocks.Add(new ProductStock { WarehouseName = "Kho HN", QuantityOnHand = 100, BatchNumber = "BATCH-003" });

            products.Add(p3);

            // --- SP 4: Áo Thun Trắng ---
            var p4 = new Product
            {
                Name = "Áo Thun Basic Trắng",
                Slug = "ao-thun-trang",
                SKU = "TSHIRT-WHT-M",
                BasePrice = 150000,
                SalePrice = 129000,
                Stocks = new List<ProductStock>
                {
                    new ProductStock
                    {
                        // Không cần gán Id = 1 (DB tự sinh)
                        // Không cần gán ProductId = 1 (EF Core tự link với p1)
                        BatchNumber = "BATCH-001",
                        WarehouseName = "952",
                        QuantityOnHand = 10,
                        CreatedAt = DateTime.Now,
                        IsDeleted = false,
                        UpdatedAt = DateTime.Now,
                    }
                },
                Category = catFashion,
                Thumbnail = "https://m.yodycdn.com/fit-in/filters:format(webp)/products/ao-thun-nam-tsm5193-trang-4.jpg",
                IsActive = true,
                IsDeleted = false,
                SpecificationsJson = "{\"Chất liệu\": \"Cotton Compact\", \"Kiểu dáng\": \"Slim Fit\", \"Cổ áo\": \"Cổ tim\", \"Xuất xứ\": \"Việt Nam\"}",
                PriceUnit = "viên",
                SalesUnit = "m2"
            };
            p4.ProductImages.Add(new ProductImage { ImageUrl = p4.Thumbnail, IsMain = true, SortOrder = 1 });
            p4.AttributeValues.Add(new ProductAttributeValue { Attribute = colorAttr, Value = "Trắng" });
            p4.AttributeValues.Add(new ProductAttributeValue { Attribute = sizeAttr, Value = "M" });
            p4.Stocks.Add(new ProductStock { WarehouseName = "Kho HCM", QuantityOnHand = 50, BatchNumber = "BATCH-004" });

            products.Add(p4);

            // Lưu tất cả Products + Attributes + Images + Stocks vào DB một lần
            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }
    }
}