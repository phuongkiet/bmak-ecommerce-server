using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Cart.Queries.GetCart
{
    public class GetCartQuery
    {
        public string CartId { get; set; }
        public GetCartQuery(string cartId) => CartId = cartId;
    }
}
