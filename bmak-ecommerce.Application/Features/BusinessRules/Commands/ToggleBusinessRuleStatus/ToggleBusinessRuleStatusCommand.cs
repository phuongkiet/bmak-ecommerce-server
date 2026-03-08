namespace bmak_ecommerce.Application.Features.BusinessRules.Commands.ToggleBusinessRuleStatus
{
    public class ToggleBusinessRuleStatusCommand
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
    }
}
