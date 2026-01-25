using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Cart.Commands.AddToCart
{
    public class AddToCartCommand
    {
        public string CartId { get; set; } // FE gửi lên (Lấy từ Cookie/LocalStorage)
        public int ProductId { get; set; }
        public int Quantity { get; set; } // Số lượng cộng thêm (VD: 1)
    }
}
