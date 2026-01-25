using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Cart.Commands.UpdateCartItem
{
    public class UpdateCartItemCommand
    {
        public string CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; } // Số lượng mới (VD: Đang 5 sửa thành 10)
    }
}
