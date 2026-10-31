using Application.Interfaces;
using Application.Interfaces.Common;
using Domain.Entities;

namespace Application.Interfaces.Product;

public interface IProductAttributeRepository : IGenericRepository<ProductAttribute>
{
}