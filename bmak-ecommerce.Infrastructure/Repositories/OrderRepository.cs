using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Domain.Entities.Sales;
using bmak_ecommerce.Domain.Interfaces;
using bmak_ecommerce.Domain.Models;
using bmak_ecommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Infrastructure.Repositories
{
    [AutoRegister]

    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Order> AddAsync(Order order)
        {
            // EF Core thông minh sẽ tự lưu cả Order và danh sách OrderItems bên trong
            await _context.Orders.AddAsync(order);

            // Commit transaction
            await _context.SaveChangesAsync();

            return order; // Lúc này order.Id đã được DB sinh ra (ví dụ: 101)
        }

        public async Task<PagedList<Order>> GetOrdersWithPaginationQuery(OrderSpecParams orderSpecParams)
        {
            var query = _context.Orders
                .Include(u => u.User)
                .Include(a => a.ShippingAddress)
                .Include(oi => oi.OrderItems)
                .AsQueryable();

            if (!string.IsNullOrEmpty(orderSpecParams.Search))
            {
                var search = orderSpecParams.Search.ToLower();
                query = query.Where(o => o.OrderCode.ToLower().Contains(search));
            }

            if (orderSpecParams.UserId.HasValue)
            {
                query = query.Where(o => o.UserId == orderSpecParams.UserId);
            }

            if (orderSpecParams.FromDate.HasValue)
                query = query.Where(x => x.OrderDate >= orderSpecParams.FromDate.Value);
        
            if (orderSpecParams.ToDate.HasValue)
            {
                var toDateEndOfDay = orderSpecParams.ToDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(x => x.OrderDate <= toDateEndOfDay);
            }

            if (orderSpecParams.Status.HasValue)
                query = query.Where(x => x.Status == orderSpecParams.Status);

            query = orderSpecParams.Sort switch
            {
                "priceAsc" => query.OrderBy(x => x.TotalAmount),
                "priceDesc" => query.OrderByDescending(x => x.TotalAmount),
                "dateAsc" => query.OrderBy(x => x.OrderDate),
                _ => query.OrderByDescending(x => x.OrderDate)
            };

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((orderSpecParams.PageIndex - 1) * orderSpecParams.PageSize)
                .Take(orderSpecParams.PageSize)
                .ToListAsync();

            return new PagedList<Order>(items, totalCount, orderSpecParams.PageIndex, orderSpecParams.PageSize);
        }
    }
}
