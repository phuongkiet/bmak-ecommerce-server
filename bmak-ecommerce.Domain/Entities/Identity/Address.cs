using bmak_ecommerce.Domain.Common;
using bmak_ecommerce.Domain.Entities.Directory;
using bmak_ecommerce.Domain.Enums;

namespace bmak_ecommerce.Domain.Entities.Identity
{
    public class Address : BaseEntity
    {
        public string ReceiverName { get; set; }
        public string Phone { get; set; }
        public string Street { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
        public string ProvinceId { get; set; }
        public string WardId { get; set; }

        public AddressType Type { get; set; }

        public int UserId { get; set; }
        public virtual AppUser User { get; set; }
        public virtual Province Province { get; set; }
        public virtual Ward Ward { get; set; }
    }
}
