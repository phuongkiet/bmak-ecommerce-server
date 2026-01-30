using bmak_ecommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Domain.Entities.Directory
{
    public class Ward
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ProvinceId { get; set; }

        public Province Province { get; set; }
    }
}

