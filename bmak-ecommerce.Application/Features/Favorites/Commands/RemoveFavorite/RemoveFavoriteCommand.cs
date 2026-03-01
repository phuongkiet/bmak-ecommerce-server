namespace bmak_ecommerce.Application.Features.Favorites.Commands.RemoveFavorite
{
    public class RemoveFavoriteCommand
    {
        public int ProductId { get; set; }

        public RemoveFavoriteCommand(int productId)
        {
            ProductId = productId;
        }
    }
}
