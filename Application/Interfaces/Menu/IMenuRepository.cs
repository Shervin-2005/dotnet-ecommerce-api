using Application.Interfaces.Common;

namespace Application.Interfaces.Menu;

public interface IMenuRepository : IGenericRepository<Domain.Entities.Menu.Menu>
{
    Task<Domain.Entities.Menu.Menu?> GetByKeyAsync(string key);
}