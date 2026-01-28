using bmak_ecommerce.Application.Features.Products.Commands.CreateProduct;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderValidator()
        {
            // Validate luôn bắt buộc cho Người mua (Billing)
            RuleFor(x => x.BuyerName).NotEmpty().WithMessage("Tên người mua là bắt buộc");
            RuleFor(x => x.BuyerPhone).NotEmpty().WithMessage("SĐT người mua là bắt buộc");
            RuleFor(x => x.BillingAddress).SetValidator(new AddressValidator());

            // Validate có điều kiện cho Người nhận (Shipping)
            // Chỉ validate khi ShipToDifferentAddress == true
            When(x => x.ShipToDifferentAddress, () => {
                RuleFor(x => x.ReceiverName).NotEmpty().WithMessage("Tên người nhận là bắt buộc");
                RuleFor(x => x.ReceiverPhone).NotEmpty().WithMessage("SĐT người nhận là bắt buộc");
                RuleFor(x => x.ShippingAddress).NotNull().SetValidator(new AddressValidator()!);
            });
        }
    }

    // Validator con cho AddressDto
    public class AddressValidator : AbstractValidator<OrderAddressDto>
    {
        public AddressValidator()
        {
            RuleFor(x => x.Province).NotEmpty().WithMessage("Chưa chọn Tỉnh/Thành");
            //RuleFor(x => x.District).NotEmpty().WithMessage("Chưa chọn Quận/Huyện");
            RuleFor(x => x.Ward).NotEmpty().WithMessage("Chưa chọn Phường/Xã");
            RuleFor(x => x.SpecificAddress).NotEmpty().WithMessage("Cần nhập số nhà/tên đường");
        }
    }
}
