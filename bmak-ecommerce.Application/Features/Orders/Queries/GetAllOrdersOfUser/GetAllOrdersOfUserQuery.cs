using bmak_ecommerce.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Orders.Queries.GetAllOrdersOfUser
{
    public class GetAllOrdersOfUserQuery
    {
        public OrderSpecParams Params { get; }

        public GetAllOrdersOfUserQuery(OrderSpecParams specParams)
        {
            Params = specParams;
        }
    }
}
