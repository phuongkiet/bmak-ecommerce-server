using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Users.Commands.UpdateUser
{
    public class UpdateUserCommand
    {
        public int UserId { get; set; } // Ẩn trong route
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public List<string> Roles { get; set; }
        public bool IsActive { get; set; } // True = Active, False = Soft Deleted
    }
}
