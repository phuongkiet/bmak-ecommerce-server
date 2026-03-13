using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Domain.Entities.Catalog;
using bmak_ecommerce.Domain.Entities.Sales;
using bmak_ecommerce.Domain.Enums;
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

        public async Task<Order?> GetOrderDetailAsync(string orderCode)
        {
            return await _context.Orders
                .Include(ot => ot.OrderItems)
                .Include(u => u.User)
                .FirstOrDefaultAsync(p => p.OrderCode == orderCode);
        }

        public async Task<RevenueReportResult> GetRevenueReportAsync(DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default)
        {
            var baseQuery = BuildCompletedOrdersQuery(fromDate, toDate);

            var summary = await baseQuery
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    TotalRevenue = g.Sum(x => x.TotalAmount),
                    TotalOrders = g.Count()
                })
                .FirstOrDefaultAsync(cancellationToken);

            var revenueByDate = await baseQuery
                .GroupBy(x => x.OrderDate.Date)
                .Select(g => new RevenueByDateItem
                {
                    Date = g.Key,
                    Revenue = g.Sum(x => x.TotalAmount),
                    Orders = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToListAsync(cancellationToken);

            var totalRevenue = summary?.TotalRevenue ?? 0m;
            var totalOrders = summary?.TotalOrders ?? 0;

            return new RevenueReportResult
            {
                TotalRevenue = totalRevenue,
                TotalOrders = totalOrders,
                AverageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0m,
                RevenueByDate = revenueByDate
            };
        }

        public async Task<ProductReportResult> GetProductReportAsync(DateTime? fromDate, DateTime? toDate, int top, CancellationToken cancellationToken = default)
        {
            var safeTop = top <= 0 ? 10 : Math.Min(top, 100);

            var orderItemsQuery = BuildCompletedOrdersQuery(fromDate, toDate)
                .SelectMany(o => o.OrderItems);

            var summary = await orderItemsQuery
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    TotalProductsSold = g.Sum(x => x.QuantityOnHand),
                    TotalRevenue = g.Sum(x => x.TotalPrice)
                })
                .FirstOrDefaultAsync(cancellationToken);

            var topProducts = await orderItemsQuery
                .GroupBy(x => new { x.ProductId, x.ProductName, x.ProductSku })
                .Select(g => new ProductReportItem
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    ProductSku = g.Key.ProductSku,
                    TotalQuantity = g.Sum(x => x.QuantityOnHand),
                    Revenue = g.Sum(x => x.TotalPrice),
                    Orders = g.Select(x => x.OrderId).Distinct().Count()
                })
                .OrderByDescending(x => x.TotalQuantity)
                .ThenByDescending(x => x.Revenue)
                .Take(safeTop)
                .ToListAsync(cancellationToken);

            return new ProductReportResult
            {
                TotalProductsSold = summary?.TotalProductsSold ?? 0,
                TotalRevenue = summary?.TotalRevenue ?? 0m,
                TopProducts = topProducts
            };
        }

        private IQueryable<Order> BuildCompletedOrdersQuery(DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.Orders
                .AsNoTracking()
                .Where(x => x.Status == OrderStatus.Completed);

            if (fromDate.HasValue)
            {
                var fromDateStart = fromDate.Value.Date;
                query = query.Where(x => x.OrderDate >= fromDateStart);
            }

            if (toDate.HasValue)
            {
                var toDateEndOfDay = toDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(x => x.OrderDate <= toDateEndOfDay);
            }

            return query;
        }
    }
}
