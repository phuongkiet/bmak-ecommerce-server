using bmak_ecommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Domain.Entities.Directory
{
    public class Province
    {
        public string Id { get; set; } // Mã tỉnh/thành phố (VD: "01", "79")
        public string Name { get; set; }
        public virtual ICollection<Ward> Wards { get; set; } = new List<Ward>();
    }
}
