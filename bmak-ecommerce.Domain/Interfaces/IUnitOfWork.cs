using bmak_ecommerce.Domain.Common;

namespace bmak_ecommerce.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // THÊM 2 DÒNG NÀY
        IProductRepository Products { get; }
        IOrderRepository Orders { get; }

        IProvinceRepository Provinces { get; }
        IWardRepository Wards { get; }

        // Hàm Generic cũ của bạn vẫn giữ nguyên
        IGenericRepository<T> Repository<T>() where T : BaseEntity;

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}