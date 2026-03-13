using bmak_ecommerce.Domain.Common;
using bmak_ecommerce.Domain.Entities.Identity;

namespace bmak_ecommerce.Domain.Entities.Catalog
{
    public class ProductLevelDiscount : BaseEntity
    {
        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;

        public int UserLevelId { get; set; }
        public virtual UserLevel UserLevel { get; set; } = null!;

        public decimal DiscountPercent { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
    }
}
