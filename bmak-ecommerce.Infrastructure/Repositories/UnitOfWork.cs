// File: UnitOfWork.cs
using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Domain.Common;
using bmak_ecommerce.Domain.Entities.Catalog;
using bmak_ecommerce.Domain.Interfaces;
using bmak_ecommerce.Infrastructure.Persistence;
using bmak_ecommerce.Infrastructure.Repositories;
using System.Collections; // Import Product

[AutoRegister]

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private Hashtable _repositories;

    // Khai báo biến private để cache
    private IProductRepository _productRepository;
    private IOrderRepository _orderRepository;
    private ICartRepository _cartRepository;
    private IProvinceRepository _provinceRepository;
    private IWardRepository _wardRepository;
    private IPageRepository _pageRepository;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    // --- IMPLEMENT 2 PROPERTY MỚI ---

    // 1. Products
    public IProductRepository Products
    {
        get
        {
            // Lazy Loading: Nếu chưa có thì new, có rồi thì trả về
            return _productRepository ??= new ProductRepository(_context);
        }
    }

    // 2. Orders (Bạn cần đảm bảo đã có class OrderRepository tương tự ProductRepository)
    public IOrderRepository Orders
    {
        get
        {
            return _orderRepository ??= new OrderRepository(_context);
        }
    }

    public IProvinceRepository Provinces
    {
        get
        {
            return _provinceRepository ??= new ProvinceRepository(_context);
        }
    }

    public IWardRepository Wards
    {
        get
        {
            return _wardRepository ??= new WardRepository(_context);
        }
    }

    public IPageRepository Pages
    {
        get
        {
            return _pageRepository ??= new PageRepository(_context);
        }
    }
    // --------------------------------

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    // Giữ nguyên logic Generic cho các bảng phụ (VD: ProductTag, Attribute...)
    public IGenericRepository<T> Repository<T>() where T : BaseEntity
    {
        if (_repositories == null)
            _repositories = new Hashtable();

        var type = typeof(T).Name;

        if (!_repositories.ContainsKey(type))
        {
            var repositoryType = typeof(GenericRepository<>);
            var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), _context);
            _repositories.Add(type, repositoryInstance);
        }

        return (IGenericRepository<T>)_repositories[type];
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}