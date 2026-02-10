using bmak_ecommerce.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Common.Interfaces
{
    public interface IUserManagementService
    {
        Task<Result<int>> CreateUserAsync(string fullName, string email, string password, string phoneNumber, List<string> roles);
        Task<Result<bool>> UpdateUserAsync(string userId, string fullName, string phoneNumber, List<string> roles, bool isActive);
        Task<Result<bool>> DeleteUserAsync(string userId, bool isHardDelete);
        Task<Result<bool>> RestoreUserAsync(string userId); // Optional: Khôi phục user soft delete
        Task<Result<bool>> ChangePasswordAdminAsync(string userId, string newPassword);
    }
}
