using bmak_ecommerce.Domain.Enums;

namespace bmak_ecommerce.Application.Features.Addresses.DTOs
{
    public class AddressDto
    {
        public int Id { get; set; }
        public string ReceiverName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string ProvinceId { get; set; } = string.Empty;
        public string ProvinceName { get; set; } = string.Empty;
        public string Zone { get; set; } = string.Empty;
        public string WardId { get; set; } = string.Empty;
        public string WardName { get; set; } = string.Empty;
        public AddressType Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
