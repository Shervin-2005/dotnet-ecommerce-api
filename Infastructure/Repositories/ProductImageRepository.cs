using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Infastructure.Repositories
{
    public class ProductImageRepository : GenericRepository<ProductImage>, IProductImageRepository
    {
        public ProductImageRepository(AppDbContext context) : base(context)
        {
        }
    }
}
