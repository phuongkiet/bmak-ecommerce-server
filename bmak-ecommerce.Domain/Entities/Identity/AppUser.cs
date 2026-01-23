using bmak_ecommerce.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace bmak_ecommerce.Domain.Entities.Identity
{
    // Kế thừa IdentityUser<int> để Id là số nguyên tự tăng
    public class AppUser : IdentityUser<int>
    {
        public string FullName { get; set; }

        // Custom fields cho ngành xây dựng
        public CustomerType CustomerType { get; set; } // Lẻ / Thầu / Đại lý

        // Navigation Properties
        public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
        public virtual ICollection<UserBehaviorTracking> Trackings { get; set; } = new List<UserBehaviorTracking>();

        // Các trường audit (CreatedDate...) nếu muốn có thì tự thêm hoặc implement interface
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; }
    }
}
