using Application.Interfaces.Product;
using Domain.Entities;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class ProductAttributeRepository : GenericRepository<ProductAttribute>, IProductAttributeRepository
{
    public ProductAttributeRepository(AppDbContext context)
        : base(context)
    {
    }
}