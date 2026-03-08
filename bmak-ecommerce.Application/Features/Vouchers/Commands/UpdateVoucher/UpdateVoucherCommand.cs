using bmak_ecommerce.Domain.Enums;

namespace bmak_ecommerce.Application.Features.Vouchers.Commands.UpdateVoucher
{
    public class UpdateVoucherCommand
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal MinOrderAmount { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int UsageLimit { get; set; }
        public int PerUserLimit { get; set; } = 1;
        public bool IsActive { get; set; }
    }
}
