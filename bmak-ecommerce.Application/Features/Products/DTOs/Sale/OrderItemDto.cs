using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Products.DTOs.Sale
{
    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ProductImage { get; set; } // Nếu entity chưa có thì sau này join bảng Product để lấy
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public float QuantitySquareMeter { get; set; }

        // Tính toán sẵn để Frontend đỡ phải nhân
        public decimal TotalLineAmount => UnitPrice * Quantity;
    }
}
