using bmak_ecommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Domain.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        // Truy vấn
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();

        // Tìm kiếm có điều kiện (Expression)
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression);

        // Thao tác (Chưa lưu vào DB ngay, chỉ Add vào Context)
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        IQueryable<T> GetAllAsQueryable();
    }
}
