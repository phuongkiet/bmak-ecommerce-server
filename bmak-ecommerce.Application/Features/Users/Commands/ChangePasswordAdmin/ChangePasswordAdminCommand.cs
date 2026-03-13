namespace bmak_ecommerce.Application.Features.Users.Commands.ChangePasswordAdmin
{
    public class ChangePasswordAdminCommand
    {
        public int UserId { get; set; }
        public string NewPassword { get; set; }
    }
}
