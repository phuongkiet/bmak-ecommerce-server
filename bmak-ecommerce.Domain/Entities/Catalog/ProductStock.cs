using bmak_ecommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Domain.Entities.Catalog
{
    public class ProductStock : BaseEntity
    {
        public string WarehouseName { get; set; }
        public string BatchNumber { get; set; } // Số lô (Quan trọng cho gạch)
        public float QuantityOnHand { get; set; }

        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
}
