using Application.Interfaces.Common;
using Domain.Entities;

namespace Application.Interfaces.Cart;

public interface ICartItemRepository: IGenericRepository<CartItem>
{
}