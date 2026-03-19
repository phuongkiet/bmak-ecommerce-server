using bmak_ecommerce.Domain.Enums;

namespace bmak_ecommerce.Application.Features.Addresses.Commands.UpdateAddress
{
    public class UpdateAddressCommand
    {
        public int Id { get; set; }
        public string ReceiverName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string Lat { get; set; }
        public string Lng { get; set; }
        public string ProvinceId { get; set; } = string.Empty;
        public string WardId { get; set; } = string.Empty;
        public AddressType Type { get; set; }
    }
}
