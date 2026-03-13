namespace bmak_ecommerce.Application.Features.Users.Dtos
{
    public class UserLevelDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Rank { get; set; }
        public bool IsActive { get; set; }
    }
}
