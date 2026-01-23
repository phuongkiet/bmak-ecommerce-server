using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Common.Models
{
    public class OrderCreatedEvent
    {
        public int OrderId { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string CustomerEmail { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
