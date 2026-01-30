using bmak_ecommerce.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Wards.Queries
{
    public class GetWardQuery
    {
        public WardSpecParams Params { get; set; }

        public GetWardQuery(WardSpecParams specParams)
        {
            Params = specParams;
        }
    }
}
