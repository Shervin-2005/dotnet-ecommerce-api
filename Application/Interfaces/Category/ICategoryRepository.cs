using Application.DTOs.Common;
using Application.Interfaces.Common;

namespace Application.Interfaces.Category
{
    public interface ICategoryRepository : IGenericRepository<Domain.Entities.Category>
    {
        Task<PagedResult<Domain.Entities.Category>> GetAllPagedAsync(PaginationRequest request, CancellationToken cancellationToken = default);
    }
}
