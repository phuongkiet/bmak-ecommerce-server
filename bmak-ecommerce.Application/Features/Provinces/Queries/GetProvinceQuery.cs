using bmak_ecommerce.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Provinces.Queries
{
    public class GetProvinceQuery
    {
        public ProvinceSpecParams Params { get; set; }

        public GetProvinceQuery(ProvinceSpecParams specParams)
        {
            Params = specParams;
        }
    }
}
