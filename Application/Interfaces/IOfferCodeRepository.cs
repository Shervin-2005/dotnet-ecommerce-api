using Domain.Entities;

namespace Application.Interfaces;

public interface IOfferCodeRepository : IGenericRepository<OfferCode>
{
    Task<OfferCode?> GetByCodeAsync(string code);

    Task<int> GetUserUsageCountAsync(
        int offerCodeId,
        int userId);

    Task<List<OfferCode>> GetAllAsync();
}