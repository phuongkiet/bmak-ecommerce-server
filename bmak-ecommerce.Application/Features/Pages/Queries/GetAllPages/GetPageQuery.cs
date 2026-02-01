using bmak_ecommerce.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Pages.Queries.GetAllPages
{
    public class GetPageQuery
    {
        public PageSpecParams Params { get; set; }

        public GetPageQuery(PageSpecParams specParams)
        {
            Params = specParams;
        }
    }
}
