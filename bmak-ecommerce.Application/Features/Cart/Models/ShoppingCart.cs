using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Cart.Models
{
    public class ShoppingCart
    {
        public string Id { get; set; } // CartId (GuestID hoặc UserId)
        public List<CartItem> Items { get; set; } = new();

        public ShoppingCart() { }
        public ShoppingCart(string id)
        {
            Id = id;
        }

        // Tổng tiền tạm tính
        public decimal TotalPrice => Items.Sum(x => x.Price * x.Quantity);

        // Tổng số m2 gạch trong giỏ (để ước lượng vận chuyển)
        public float TotalSquareMeter => Items.Sum(x => x.QuantitySquareMeter);
    }

    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductSlug { get; set; }

        public decimal Price { get; set; }      // Giá tại thời điểm thêm vào giỏ
        public decimal OriginalPrice { get; set; }

        public int Quantity { get; set; }       // Số lượng VIÊN
        public string PictureUrl { get; set; }

        // --- LOGIC GẠCH MEN ---
        public string SalesUnit { get; set; }   // "Viên"
        public string PriceUnit { get; set; }   // "m2"
        public float ConversionFactor { get; set; } // 0.36

        // Tính toán hiển thị: 10 viên * 0.36 = 3.6 m2
        public float QuantitySquareMeter => Quantity * (ConversionFactor > 0 ? ConversionFactor : 1);
    }
}
