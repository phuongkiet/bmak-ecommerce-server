namespace bmak_ecommerce.Application.Features.Users.Commands.RestoreUser
{
    public class RestoreUserCommand
    {
        public int UserId { get; set; }

        public RestoreUserCommand(int userId)
        {
            UserId = userId;
        }
    }
}
