namespace bmak_ecommerce.Application.Features.Vouchers.Commands.ToggleVoucherStatus
{
    public class ToggleVoucherStatusCommand
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
    }
}
