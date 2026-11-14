using Application.DTOs.Common;
using Application.Interfaces.Common;

namespace Application.Interfaces.Brand
{
    public interface IBrandRepository : IGenericRepository<Domain.Entities.Brand>
    { 
        Task<PagedResult<Domain.Entities.Brand>> GetAllPagedAsync(PaginationRequest request, CancellationToken cancellationToken = default);
    }
}
