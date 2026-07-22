using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Infastructure.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context)
        {
        }
    }
}
