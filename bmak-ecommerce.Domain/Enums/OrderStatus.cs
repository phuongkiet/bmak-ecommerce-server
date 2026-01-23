using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Domain.Enums
{
    public enum OrderStatus
    {
        Pending = 1,        // Mới đặt, chờ xác nhận
        Confirmed = 2,      // Đã xác nhận, đang soạn hàng
        Shipping = 3,       // Đang giao
        Completed = 4,      // Hoàn thành
        Cancelled = 5,      // Hủy
        Returned = 6        // Trả hàng
    }
}
