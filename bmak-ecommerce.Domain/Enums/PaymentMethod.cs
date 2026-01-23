using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Domain.Enums
{
    public enum PaymentMethod
    {
        COD = 1,            // Tiền mặt khi nhận hàng
        BankTransfer = 2,   // Chuyển khoản
        OnlineGateway = 3,  // Cổng thanh toán (Momo, VNPay...)
        Debt = 4            // Công nợ (Dành cho đại lý/nhà thầu thân thiết)
    }
}
