using bmak_ecommerce.Domain.Common;
using bmak_ecommerce.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Domain.Entities.Identity
{
    public class Address : BaseEntity
    {
        public string ReceiverName { get; set; }
        public string Phone { get; set; }
        public string Street { get; set; }
        public string Ward { get; set; }
        public string District { get; set; }
        public string City { get; set; }

        public AddressType Type { get; set; }

        public int UserId { get; set; }
        public virtual AppUser User { get; set; }
    }
}
