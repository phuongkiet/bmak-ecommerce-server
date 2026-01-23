using bmak_ecommerce.Domain.Entities.Sales;
using bmak_ecommerce.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Domain.Interfaces
{
    public interface IOrderRepository
    {
        // Hàm thêm đơn hàng, trả về Order đã có ID
        Task<Order> AddAsync(Order order);
        Task<PagedList<Order>> GetOrdersWithPaginationQuery(OrderSpecParams orderSpecParams);
    }
}
