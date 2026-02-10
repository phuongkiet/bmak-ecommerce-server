using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Users.Commands.DeleteUser
{
    public class DeleteUserCommand
    {
        public int UserId { get; set; }
        public bool IsHardDelete { get; set; } = false; // Mặc định là Soft Delete

        public DeleteUserCommand(int id, bool hardDelete)
        {
            UserId = id;
            IsHardDelete = hardDelete;
        }
    }
}
