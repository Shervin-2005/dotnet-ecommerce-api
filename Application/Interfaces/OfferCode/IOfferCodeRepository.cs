using Application.DTOs.Common;
using Application.Interfaces.Common;

namespace Application.Interfaces.OfferCode;

public interface IOfferCodeRepository : IGenericRepository<Domain.Entities.OfferCode>
{
    Task<Domain.Entities.OfferCode?> GetByCodeAsync(string code);

    Task<int> GetUserUsageCountAsync(int offerCodeId, int userId);

    Task<PagedResult<Domain.Entities.OfferCode>> GetAllPagedAsync(PaginationRequest request, CancellationToken cancellationToken = default);
}