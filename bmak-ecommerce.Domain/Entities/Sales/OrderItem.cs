using bmak_ecommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Domain.Entities.Sales
{
    public class OrderItem : BaseEntity
    {
        public int Quantity { get; set; } // Số lượng đơn vị bán (10 Thùng)
        public float QuantitySquareMeter { get; set; } // Quy đổi ra m2 (14.4m2) - Lưu để đối chiếu

        public decimal UnitPrice { get; set; } // Giá lúc mua
        public decimal TotalPrice { get; set; }

        // Navigation
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public virtual Domain.Entities.Catalog.Product Product { get; set; }
    }
}
