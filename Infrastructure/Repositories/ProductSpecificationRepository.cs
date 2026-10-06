using Application.Interfaces;
using Infrastructure.Data;
using Domain.Entities;

namespace Infrastructure.Repositories;

public class ProductSpecificationRepository : GenericRepository<ProductSpecification>, IProductSpecificationRepository
{
    public ProductSpecificationRepository(AppDbContext context) : base(context)
    {
    }
}