using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Infrastructure.SignalR
{
    [Authorize(Roles = "Admin")]
    public class AdminNotificationHub : Hub
    {

    }
}
