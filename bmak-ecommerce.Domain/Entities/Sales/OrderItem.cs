using bmak_ecommerce.Domain.Common;
using bmak_ecommerce.Domain.Entities.Catalog; // Import Product namespace
using System.ComponentModel.DataAnnotations.Schema; // Dùng cho [NotMapped] nếu cần

namespace bmak_ecommerce.Domain.Entities.Sales
{
    public class OrderItem : BaseEntity
    {
        // ==========================================
        // 1. NAVIGATION (KHOÁ NGOẠI)
        // ==========================================
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }

        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        // ==========================================
        // 2. SNAPSHOT (DỮ LIỆU LỊCH SỬ)
        // Lưu cứng lại để khi Product gốc thay đổi, đơn hàng cũ không bị sai
        // ==========================================

        public string ProductName { get; set; } // Tên lúc mua

        public string ProductSku { get; set; }  // --- MỚI: Mã SKU lúc mua ---

        public string? ProductImage { get; set; } // --- MỚI: Ảnh lúc mua (Fix lỗi) ---

        public decimal Price { get; set; }      // --- ĐỔI TÊN: UnitPrice -> Price (Fix lỗi) ---
                                                // Giá đơn vị lúc mua (sau khi trừ KM nếu có)

        // ==========================================
        // 3. SỐ LƯỢNG & TÍNH TOÁN
        // ==========================================

        public float QuantityOnHand { get; set; }

        public float QuantitySquareMeter { get; set; } // Quy đổi m2 (ví dụ: 14.4m2) - Logic riêng của bạn

        public decimal TotalPrice { get; set; } // Tổng tiền dòng này = Price * Quantity
    }
}