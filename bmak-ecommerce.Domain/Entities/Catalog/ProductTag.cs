using bmak_ecommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Domain.Entities.Catalog
{
    /// <summary>
    /// Join table cho quan hệ Many-to-Many giữa Product và Tag
    /// </summary>
    public class ProductTag : BaseEntity
    {
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        public int TagId { get; set; }
        public virtual Tag Tag { get; set; }
    }
}


