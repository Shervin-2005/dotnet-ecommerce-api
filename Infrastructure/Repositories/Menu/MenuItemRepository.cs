using Application.Interfaces.Menu;
using Domain.Entities.Menu;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Menu;

public class MenuItemRepository : GenericRepository<MenuItem>, IMenuItemRepository
{
    public MenuItemRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<MenuItem>> GetByMenuIdAsync(int menuId)
    {
        return await _dbSet.Where(x => x.MenuId == menuId).AsNoTracking().ToListAsync();
    }
}