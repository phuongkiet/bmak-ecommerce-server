using bmak_ecommerce.Domain.Entities.Identity;

namespace bmak_ecommerce.Domain.Interfaces
{
    public interface IFavoriteRepository
    {
        Task<UserFavoriteProduct?> GetAsync(int userId, int productId);
        Task<List<UserFavoriteProduct>> GetByUserIdAsync(int userId);
        Task AddAsync(UserFavoriteProduct favorite);
        void Remove(UserFavoriteProduct favorite);
    }
}
