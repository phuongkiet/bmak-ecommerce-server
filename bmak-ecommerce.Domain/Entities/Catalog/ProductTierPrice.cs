using bmak_ecommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Domain.Entities.Catalog
{
    public class ProductTierPrice : BaseEntity
    {
        public float MinQuantity { get; set; } // Mua > 100m2
        public decimal Price { get; set; }     // Giá giảm còn X

        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
}
