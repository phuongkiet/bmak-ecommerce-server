using bmak_ecommerce.Application.Features.Orders.Commands.CreateOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Products.DTOs.Sale
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }

        // Trạng thái & Thanh toán
        public string Status { get; set; } = string.Empty; // Trả về String (Pending) thay vì Int (0) để FE dễ hiển thị
        public string PaymentMethod { get; set; } = string.Empty;

        // Thông tin tài chính
        public decimal SubTotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Note { get; set; }

        // Thông tin khách hàng (Flattening - Làm phẳng dữ liệu từ bảng User)
        public int UserId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;

        // Thông tin giao hàng (Map từ bảng Address)
        public string ShippingAddress { get; set; } = string.Empty; // Có thể nối chuỗi: "123 Đường A, Quận B, TP.HCM"

        // Danh sách sản phẩm (Quan trọng nhất của trang Detail)
        public List<OrderItemDto> OrderItems { get; set; } = new();
    }
}
