namespace Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        IProductImageRepository ProductImages { get; }
        IProductSpecificationRepository ProductSpecifications { get; }
        IProductVariantRepository ProductVariants { get; }
        IProductAttributeRepository ProductAttributes { get; }
        IProductAttributeValueRepository ProductAttributeValues { get; }
        IBrandRepository Brands { get; }
        ICategoryRepository Categories { get; }
        IUserRepository Users {  get; }
        IUserAddressRepository UserAddresses { get; }
        IOtpVerificationRepository OtpVerifications { get; }
        IRefreshTokenRepository? RefreshTokens {  get; }
        IReviewRepository Reviews { get; }
        ICartRepository Carts { get; }
        ICartItemRepository CartItems { get; }
        IOrderRepository Orders { get; }
        IPaymentRepository Payments { get; }
        Task<int> SaveChangesAsync();
    }
}