using bmak_ecommerce.Domain.Common;
using bmak_ecommerce.Domain.Entities.Identity;
using bmak_ecommerce.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Domain.Entities.Sales
{
    public class Order : BaseEntity
    {
        public string OrderCode { get; set; } // Mã đơn: ORD-2025-001
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public PaymentMethod PaymentMethod { get; set; }

        // Tài chính
        public decimal SubTotal { get; set; } // Tổng tiền hàng
        public decimal ShippingFee { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; } // (Sub + Ship - Discount)

        public string Note { get; set; } // Ghi chú giao hàng

        // Navigation
        public int UserId { get; set; }
        public virtual AppUser User { get; set; }

        public int? ShippingAddressId { get; set; }
        public virtual Address ShippingAddress { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
