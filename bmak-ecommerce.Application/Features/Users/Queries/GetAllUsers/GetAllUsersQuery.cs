using bmak_ecommerce.Domain.Models;

namespace bmak_ecommerce.Application.Features.Users.Queries.GetAllUsers
{
    public class GetAllUsersQuery
    {
        public UserSpecParams Params { get; set; }
        public GetAllUsersQuery(UserSpecParams par) { Params = par; }
    }
}
