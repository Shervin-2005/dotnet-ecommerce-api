using Application.Interfaces.Brand;
using Application.Interfaces.Cart;
using Application.Interfaces.Category;
using Application.Interfaces.OfferCode;
using Application.Interfaces.Order;
using Application.Interfaces.Payment;
using Application.Interfaces.Product;
using Application.Interfaces.Review;
using Application.Interfaces.SupportChat;
using Application.Interfaces.UserAndAuth;

namespace Application.Interfaces.Common
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
        IBrandGroupRepository BrandGroups { get; }
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
        IOfferCodeRepository OfferCodes { get; }
        ISupportConversationRepository SupportConversations { get; }
        ISupportMessageRepository SupportMessages { get; }
        Task<int> SaveChangesAsync();
    }
}