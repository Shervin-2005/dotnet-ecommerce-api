using Application.Interfaces.Common;
using Domain.Entities.Menu;

namespace Application.Interfaces.Menu;

    public interface IMenuItemRepository : IGenericRepository<MenuItem>
    {
        Task<List<MenuItem>> GetByMenuIdAsync(int menuId);
    }