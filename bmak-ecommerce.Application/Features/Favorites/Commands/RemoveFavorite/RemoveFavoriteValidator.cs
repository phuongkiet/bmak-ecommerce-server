using FluentValidation;

namespace bmak_ecommerce.Application.Features.Favorites.Commands.RemoveFavorite
{
    public class RemoveFavoriteValidator : AbstractValidator<RemoveFavoriteCommand>
    {
        public RemoveFavoriteValidator()
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0)
                .WithMessage("ProductId không hợp lệ.");
        }
    }
}
