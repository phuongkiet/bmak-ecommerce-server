using bmak_ecommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Domain.Entities.Catalog
{
    public class ProductAttributeValue : BaseEntity
    {
        public string Value { get; set; } // "60x60", "Matt"
        public string ExtraData { get; set; } // Mã màu Hex hoặc Icon

        // Navigation
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        public int AttributeId { get; set; }
        public virtual ProductAttribute Attribute { get; set; }
    }
}
