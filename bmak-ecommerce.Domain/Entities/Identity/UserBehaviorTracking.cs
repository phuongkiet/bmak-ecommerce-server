using bmak_ecommerce.Domain.Common;
using bmak_ecommerce.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Domain.Entities.Identity
{
    public class UserBehaviorTracking : BaseEntity
    {
        public string SessionId { get; set; } // Dùng cho khách vãng lai
        public TrackingActionType ActionType { get; set; }

        // Metadata lưu thông tin filter: {"color": "grey", "size": "60x60"}
        public string MetadataJson { get; set; }

        public int? UserId { get; set; }
        public virtual AppUser User { get; set; }

        public int? ProductId { get; set; }
    }
}
