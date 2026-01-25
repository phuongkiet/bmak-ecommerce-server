using bmak_ecommerce.Application.Features.Cart.Commands.DeleteCartItem;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Cart.Commands.ClearCart
{
    public class ClearCartValidator : AbstractValidator<ClearCartCommand>
    {
        public ClearCartValidator()
        {
            RuleFor(x => x.CartId).NotEmpty().WithMessage("CartId không được để trống");
        }
    }
}
