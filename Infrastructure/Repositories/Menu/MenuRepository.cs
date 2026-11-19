using Application.Interfaces.Menu;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Menu;

public class MenuRepository : GenericRepository<Domain.Entities.Menu.Menu>, IMenuRepository
{
    public MenuRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Domain.Entities.Menu.Menu?> GetByKeyAsync(string key) 
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.Key == key);
    }
}