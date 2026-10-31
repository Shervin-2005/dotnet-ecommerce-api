using Application.Interfaces.Common;
using Domain.Entities;

namespace Application.Interfaces.OfferCode;

public interface IOfferCodeRepository : IGenericRepository<Domain.Entities.OfferCode>
{
    Task<Domain.Entities.OfferCode?> GetByCodeAsync(string code);

    Task<int> GetUserUsageCountAsync(
        int offerCodeId,
        int userId);

    Task<List<Domain.Entities.OfferCode>> GetAllAsync();
}