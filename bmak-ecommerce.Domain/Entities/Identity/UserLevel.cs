using bmak_ecommerce.Domain.Common;
using bmak_ecommerce.Domain.Entities.Catalog;

namespace bmak_ecommerce.Domain.Entities.Identity
{
    public class UserLevel : BaseEntity
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Rank { get; set; }
        public bool IsActive { get; set; } = true;

        public virtual ICollection<AppUser> Users { get; set; } = new List<AppUser>();
        public virtual ICollection<ProductLevelDiscount> ProductLevelDiscounts { get; set; } = new List<ProductLevelDiscount>();
    }
}
