using bmak_ecommerce.Domain.Common;

namespace bmak_ecommerce.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // THÊM 2 DÒNG NÀY
        IProductRepository Products { get; }
        IOrderRepository Orders { get; }
        IFavoriteRepository Favorites { get; }

        IProvinceRepository Provinces { get; }
        IWardRepository Wards { get; }
        IPageRepository Pages { get; }

        // Hàm Generic cũ của bạn vẫn giữ nguyên
        IGenericRepository<T> Repository<T>() where T : class;

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}