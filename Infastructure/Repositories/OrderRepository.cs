using Application.Interfaces;
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
            .FirstOrDefaultAsync(o => o.OrderId == orderId);

    public async Task<List<Order>> GetByUserIdAsync(int userId) =>
        await _dbSet
            .Where(o => o.UserId == userId)
            .Include(o => o.Items)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

    public async Task<List<Order>> GetAllWithDetailsAsync() =>
        await _dbSet
            .Include(o => o.Items)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
}