using bmak_ecommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Domain.Entities.Catalog
{
    public class ProductAttribute : BaseEntity
    {
        public string Name { get; set; } // "Kích thước", "Bề mặt"
        public string Code { get; set; } // "SIZE", "SURFACE"
        public virtual ICollection<ProductAttributeValue> Values { get; set; }
    }
}
