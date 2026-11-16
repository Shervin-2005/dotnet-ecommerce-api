using Application.DTOs.FlashSale;

namespace Application.Interfaces.FlashSale;

public interface IFlashSaleService
{
    Task<FlashSaleDto> CreateAsync(CreateFlashSaleDto dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(int flashSaleId, UpdateFlashSaleDto dto, CancellationToken cancellationToken = default);
    Task<FlashSaleDto?> GetByIdAsync(int flashSaleId, CancellationToken cancellationToken = default);
    Task AddItemsAsync(int flashSaleId, AddFlashSaleItemDto dto, CancellationToken cancellationToken = default);
    Task RemoveItemAsync(int flashSaleId, int flashSaleItemId, CancellationToken cancellationToken = default);
}