using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Infastructure.Repositories
{
    public class BrandRepository : GenericRepository<Brand> , IBrandRepository
    {
        public BrandRepository(AppDbContext context) : base(context)
        {
        }
    }
}
