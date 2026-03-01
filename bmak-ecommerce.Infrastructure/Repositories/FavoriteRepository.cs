using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Domain.Entities.Identity;
using bmak_ecommerce.Domain.Interfaces;
using bmak_ecommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace bmak_ecommerce.Infrastructure.Repositories
{
    [AutoRegister]
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly AppDbContext _context;

        public FavoriteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserFavoriteProduct?> GetAsync(int userId, int productId)
        {
            return await _context.UserFavoriteProducts
                .FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == productId);
        }

        public async Task<List<UserFavoriteProduct>> GetByUserIdAsync(int userId)
        {
            return await _context.UserFavoriteProducts
                .AsNoTracking()
                .Include(x => x.Product)
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(UserFavoriteProduct favorite)
        {
            await _context.UserFavoriteProducts.AddAsync(favorite);
        }

        public void Remove(UserFavoriteProduct favorite)
        {
            _context.UserFavoriteProducts.Remove(favorite);
        }
    }
}
