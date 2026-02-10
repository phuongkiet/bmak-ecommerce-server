using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Infrastructure.Services
{
    [AutoRegister]

    public class UserManagementService : IUserManagementService
    {
        private readonly UserManager<AppUser> _userManager;

        public UserManagementService(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<int>> CreateUserAsync(string fullName, string email, string password, string phoneNumber, List<string> roles)
        {
            var userExists = await _userManager.FindByEmailAsync(email);
            if (userExists != null) return Result<int>.Failure("Email đã tồn tại.");

            var user = new AppUser
            {
                UserName = email,
                Email = email,
                FullName = fullName,
                PhoneNumber = phoneNumber,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false,
                EmailConfirmed = true // Admin tạo thì auto confirm
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                return Result<int>.Failure(string.Join(", ", result.Errors.Select(e => e.Description)));

            if (roles != null && roles.Any())
            {
                await _userManager.AddToRolesAsync(user, roles);
            }

            return Result<int>.Success(user.Id);
        }

        public async Task<Result<bool>> UpdateUserAsync(string userId, string fullName, string phoneNumber, List<string> roles, bool isActive)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Result<bool>.Failure("Không tìm thấy người dùng.");

            user.FullName = fullName;
            user.PhoneNumber = phoneNumber;
            user.IsDeleted = !isActive; // Nếu isActive = false -> IsDeleted = true (Tùy logic)

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) return Result<bool>.Failure("Cập nhật không thành công.");

            // Update Roles
            var currentRoles = await _userManager.GetRolesAsync(user);
            var rolesToAdd = roles.Except(currentRoles);
            var rolesToRemove = currentRoles.Except(roles);

            await _userManager.AddToRolesAsync(user, rolesToAdd);
            await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> DeleteUserAsync(string userId, bool isHardDelete)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Result<bool>.Failure("Không tìm thấy người dùng.");

            if (isHardDelete)
            {
                // Xóa vĩnh viễn khỏi DB
                var result = await _userManager.DeleteAsync(user);
                return result.Succeeded ? Result<bool>.Success(true) : Result<bool>.Failure("Xóa cứng không thành công.");
            }
            else
            {
                // Soft Delete
                user.IsDeleted = true;
                // Có thể đổi SecurityStamp để force logout
                await _userManager.UpdateSecurityStampAsync(user);
                var result = await _userManager.UpdateAsync(user);
                return result.Succeeded ? Result<bool>.Success(true) : Result<bool>.Failure("Xóa tạm không thành công.");
            }
        }

        public async Task<Result<bool>> RestoreUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Result<bool>.Failure("Không tìm thấy người dùng.");

            user.IsDeleted = false;
            var result = await _userManager.UpdateAsync(user);
            return Result<bool>.Success(result.Succeeded);
        }

        public async Task<Result<bool>> ChangePasswordAdminAsync(string userId, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Result<bool>.Failure("Không tìm thấy người dùng.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            return Result<bool>.Success(result.Succeeded);
        }
    }
}
