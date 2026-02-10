using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Authentications.DTOs;
using bmak_ecommerce.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace bmak_ecommerce.Infrastructure.Services
{
    [AutoRegister]

    public class IdentityService : IIdentityService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;

        public IdentityService(UserManager<AppUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<Result<AuthResponse>> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, password))
                return Result<AuthResponse>.Failure("Tài khoản hoặc mật khẩu không đúng.");

            // Tạo JWT Token (Logic chi tiết xem lại tin nhắn trước)
            var token = await GenerateJwtToken(user);
            var roles = await _userManager.GetRolesAsync(user);

            return Result<AuthResponse>.Success(new AuthResponse
            {
                Id = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FullName = user.FullName,
                Token = token,
                Roles = roles.ToList()
            });
        }

        public async Task<Result<AuthResponse>> RefreshTokenAsync(TokenRequest request)
        {
            var accessToken = request.AccessToken;
            var refreshToken = request.RefreshToken;

            // 1. Trích xuất thông tin User từ Access Token (dù đã hết hạn)
            var principal = GetPrincipalFromExpiredToken(accessToken);
            if (principal == null)
                return Result<AuthResponse>.Failure("Invalid access token or refresh token");

            var email = principal.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return Result<AuthResponse>.Failure("Invalid access token or refresh token");
            }

            // 2. Tạo cặp Token mới
            var newAccessToken = await GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();

            // 3. Cập nhật DB (Thu hồi cái cũ, lưu cái mới - Token Rotation)
            user.RefreshToken = newRefreshToken;
            await _userManager.UpdateAsync(user);

            var roles = await _userManager.GetRolesAsync(user);

            return Result<AuthResponse>.Success(new AuthResponse
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Token = newAccessToken,
                RefreshToken = newRefreshToken,
                Roles = roles.ToList()
            });
        }

        public async Task<Result<string>> RegisterAsync(string fullName, string phoneNumber, string email, string password)
        {
            var user = new AppUser { UserName = email, Email = email, FullName = fullName, PhoneNumber = phoneNumber };
            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
                return Result<string>.Failure(string.Join(", ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, "Customer");
            return Result<string>.Success(user.Id.ToString());
        }

        public async Task<Result<bool>> LogoutAsync(int userId)
        {
            if (userId <= 0) Result<bool>.Failure("Logout failed");
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return Result<bool>.Failure("User not found");

            // Xóa Refresh Token và set ngày hết hạn về quá khứ
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = DateTime.MinValue;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
                return Result<bool>.Success(true);

            return Result<bool>.Failure("Logout failed");
        }

        // --- HELPER: Tạo Refresh Token ngẫu nhiên ---
        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        // --- HELPER: Lấy thông tin từ Token hết hạn ---
        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"])),
                ValidateLifetime = false // Quan trọng: Bỏ qua kiểm tra hết hạn để đọc được info
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        private async Task<string> GenerateJwtToken(AppUser user)
        {
            // 1. Lấy cấu hình từ appsettings.json
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["Secret"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expirationInMinutes = double.Parse(jwtSettings["ExpirationInMinutes"]);

            // 2. Tạo danh sách Claims (Dữ liệu chứa trong Token)
            var claims = new List<Claim>
            {
                // Sub (Subject) thường là ID của user
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // ID riêng của Token
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Map vào User.Identity.Name
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            // 3. Lấy Roles của User từ DB và add vào Claims
            // Bước này QUAN TRỌNG để làm chức năng phân quyền [Authorize(Roles = "Admin")]
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // 4. Tạo Chữ ký bảo mật (Signing Key)
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 5. Tạo Token Object
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationInMinutes),
                signingCredentials: creds
            );

            // 6. Trả về chuỗi Token
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
