using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Vouchers.DTOs
{
    public class VoucherResponseDto
    {
        public string Code { get; set; } = string.Empty;
        public decimal DiscountAmount { get; set; } // Số tiền được giảm thực tế
        public string Message { get; set; } = string.Empty; // Lời chúc mừng (VD: "Áp dụng mã thành công!")
    }
}
