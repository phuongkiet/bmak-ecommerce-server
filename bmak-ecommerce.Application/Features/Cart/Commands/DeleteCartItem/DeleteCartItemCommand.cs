using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Cart.Commands.DeleteCartItem
{
    public class DeleteCartItemCommand
    {
        public string CartId { get; set; }
        public int ProductId { get; set; }
    }
}
