using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Cart.Commands.UpdateCartItem
{
    public class DeleteCartItemValidator : AbstractValidator<UpdateCartItemCommand>
    {
        public DeleteCartItemValidator()
        {
            RuleFor(x => x.CartId).NotEmpty().WithMessage("CartId không được để trống");
            RuleFor(x => x.ProductId).GreaterThan(0).WithMessage("ProductId không hợp lệ");
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Số lượng phải lớn hơn 0");
        }
    }
}
