using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Orders.Queries.GetOrderById
{
    public class GetOrderByIdQuery
    {
        public string OrderCode { get; set; }

        public GetOrderByIdQuery(string orderCode)
        {
            OrderCode = orderCode;
        }
    }
}
