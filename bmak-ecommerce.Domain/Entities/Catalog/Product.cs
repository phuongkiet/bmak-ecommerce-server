using bmak_ecommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Domain.Entities.Catalog
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string SKU { get; set; } // Mã sản phẩm
        public string Slug { get; set; }

        // Giá & Đơn vị
        public decimal BasePrice { get; set; } // Giá bán thường (giá gốc)
        public decimal SalePrice { get; set; } // Giá giảm (nếu có)
        public string SalesUnit { get; set; }  // "Thùng", "Hộp"
        public string PriceUnit { get; set; }  // "m2", "Viên"

        // Thông tin giảm giá
        public DateTime? SaleStartDate { get; set; } // Ngày bắt đầu giảm giá
        public DateTime? SaleEndDate { get; set; } // Ngày kết thúc giảm giá

        // QUAN TRỌNG: 1 Thùng = 1.44m2
        public float ConversionFactor { get; set; }

        public float Weight { get; set; } // Cân nặng (kg) để tính ship

        // MySQL JSON Column
        public string SpecificationsJson { get; set; }
        public bool IsActive { get; set; }

        // Navigation
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        // 1. Ảnh đại diện (Lưu thừa 1 chút để query list cho nhanh)
        public string? Thumbnail { get; set; }

        // 2. Danh sách ảnh chi tiết (Relation)
        public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

        public virtual ICollection<ProductAttributeValue> AttributeValues { get; set; } = new List<ProductAttributeValue>();
        public virtual ICollection<ProductTierPrice> TierPrices { get; set; } = new List<ProductTierPrice>();
        public virtual ICollection<ProductStock> Stocks { get; set; } = new List<ProductStock>();
        
        // Many-to-Many với Tag
        public virtual ICollection<ProductTag> ProductTags { get; set; } = new List<ProductTag>();
    }
}
