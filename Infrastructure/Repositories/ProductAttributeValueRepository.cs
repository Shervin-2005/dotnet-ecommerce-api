using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class ProductAttributeValueRepository : GenericRepository<ProductAttributeValue>, IProductAttributeValueRepository
{
    public ProductAttributeValueRepository(AppDbContext context) : base(context)
    {
    }
}