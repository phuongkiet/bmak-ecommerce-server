using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Pages.Queries.GetPageDetail
{
    public class GetPageDetailQuery
    {
        public string Slug { get; set; }

        public GetPageDetailQuery(string slug)
        {
            Slug = slug;
        }
    }
}
