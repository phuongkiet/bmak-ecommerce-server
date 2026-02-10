using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Authentications.Commands.Logout
{
    public class LogoutCommand
    {
        public int UserId { get; set; }

        public LogoutCommand(int userId)
        {
            UserId = userId;
        }
    }
}
