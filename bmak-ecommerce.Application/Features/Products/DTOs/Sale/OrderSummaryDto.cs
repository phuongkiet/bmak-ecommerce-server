using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Products.DTOs.Sale
{
    public class OrderSummaryDto
    {
        public int Id { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        // Chỉ đếm số lượng, không load data
        public int ItemCount { get; set; }
    }
}
