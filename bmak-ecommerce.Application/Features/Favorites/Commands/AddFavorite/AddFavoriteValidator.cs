using FluentValidation;

namespace bmak_ecommerce.Application.Features.Favorites.Commands.AddFavorite
{
    public class AddFavoriteValidator : AbstractValidator<AddFavoriteCommand>
    {
        public AddFavoriteValidator()
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0)
                .WithMessage("ProductId không hợp lệ.");
        }
    }
}
