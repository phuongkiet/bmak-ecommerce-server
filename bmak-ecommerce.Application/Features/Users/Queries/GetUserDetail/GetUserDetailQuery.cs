using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Users.Queries.GetUserDetail
{
    public class GetUserDetailQuery
    {
        public int UserId { get; set; }
        public GetUserDetailQuery(int id) { UserId = id; }
    }
}
