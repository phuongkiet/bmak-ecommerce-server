using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Pages.Queries.GetPageDetail
{
    public class GetPageDetailQuery
    {
        public int? Id { get; set; }
        public string? Slug { get; set; } // Thêm trường này

        public GetPageDetailQuery(int id) { Id = id; }
        public GetPageDetailQuery(string slug) { Slug = slug; }
    }
}
