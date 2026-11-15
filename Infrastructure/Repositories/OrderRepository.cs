using Application.DTOs.Common;
using Application.Interfaces.Order;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    public OrderRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Order?> GetByIdWithDetailsAsync(int orderId) =>
        await _dbSet
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Include(o => o.Items)
            .ThenInclude(i => i.ProductVariant)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);

    public async Task<PagedResult<Order>> GetByUserIdPagedWithDetailsAsync(int userId, PaginationRequest request, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(o => o.UserId == userId);

        return await GetPagedOrdersAsync(query, request, cancellationToken);
    }

    public async Task<PagedResult<Order>> GetAllPagedWithDetailsAsync(PaginationRequest request, CancellationToken cancellationToken = default)
    {
        return await GetPagedOrdersAsync(_dbSet, request, cancellationToken);
    }

    private async Task<PagedResult<Order>> GetPagedOrdersAsync(IQueryable<Order> query, PaginationRequest request, CancellationToken cancellationToken)
    {
        var totalCount = await query.CountAsync(cancellationToken);

        var skip = (int)Math.Min(
            ((long)request.Page - 1) * request.PageSize,
            int.MaxValue);

        var pageIds = await query
            .OrderByDescending(o => o.CreatedAt)
            .ThenByDescending(o => o.OrderId)
            .Skip(skip)
            .Take(request.PageSize)
            .Select(o => o.OrderId)
            .ToListAsync(cancellationToken);

        var orders = await _dbSet
            .AsNoTracking()
            .AsSplitQuery()
            .Where(o => pageIds.Contains(o.OrderId))
            .Include(o => o.Items)
            .ThenInclude(i => i.ProductVariant)
            .ToListAsync(cancellationToken);

        var positions = pageIds
            .Select((id, index) => new { id, index })
            .ToDictionary(x => x.id, x => x.index);

        var orderedOrders = orders
            .OrderBy(o => positions[o.OrderId])
            .ToList();

        return new PagedResult<Order>
        {
            Items = orderedOrders,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<List<Order>> GetPendingOrdersOlderThanAsync(DateTime expirationTime)
    {
        return await _dbSet
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Include(o => o.Items)
            .ThenInclude(i => i.ProductVariant)
            .Where(o =>
                o.Status == Domain.Enums.OrderStatus.Pending &&
                o.CreatedAt <= expirationTime)
            .ToListAsync();
    }
}