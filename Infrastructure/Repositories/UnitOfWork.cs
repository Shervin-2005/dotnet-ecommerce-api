using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Repositories;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IProductRepository? _products;
        private IProductImageRepository _productImages;
        private IProductSpecificationRepository?  _productSpecifications;
        private IProductVariantRepository? _productVariants;
        private IProductAttributeRepository? _productAttributes;
        private IProductAttributeValueRepository _productAttributeValues;
        private IBrandRepository _brands;
        private ICategoryRepository _categories;
        private IUserRepository _userRepository;
        private IUserAddressRepository _userAddresses;
        private IOtpVerificationRepository? _otpVerifications;
        private IRefreshTokenRepository? _refreshTokens;
        private IReviewRepository? _reviews;
        private ICartRepository? _carts;
        private ICartItemRepository? _cartItems;
        private IOrderRepository? _orders;
        private IPaymentRepository? _payments;
        private IOfferCodeRepository? _offerCodes;
        
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IProductRepository Products => _products ??= new ProductRepository(_context);
        public IProductImageRepository ProductImages => _productImages ??= new ProductImageRepository(_context);
        public IProductSpecificationRepository ProductSpecifications => _productSpecifications ??= new ProductSpecificationRepository(_context);
        public IProductAttributeRepository ProductAttributes => _productAttributes ??= new ProductAttributeRepository(_context);
        public IProductAttributeValueRepository ProductAttributeValues => _productAttributeValues ??= new ProductAttributeValueRepository(_context);
        public IProductVariantRepository ProductVariants => _productVariants ??= new ProductVariantRepository(_context);
        public IBrandRepository Brands => _brands ??= new BrandRepository(_context);
        public ICategoryRepository Categories =>_categories ??= new CategoryRepository(_context);
        public IUserRepository Users => _userRepository ??= new UserRepository(_context);
        public IUserAddressRepository  UserAddresses => _userAddresses ??= new UserAddressRepository(_context);
        public IOtpVerificationRepository OtpVerifications => _otpVerifications ??= new OtpVerificationRepository(_context);
        public IRefreshTokenRepository RefreshTokens => _refreshTokens ??= new RefreshTokenRepository(_context);
        public IReviewRepository Reviews => _reviews ??= new ReviewRepository(_context);
        public ICartRepository Carts => _carts ??= new CartRepository(_context);
        public ICartItemRepository CartItems =>  _cartItems ??= new CartItemRepository(_context);
        public IOrderRepository Orders => _orders ??= new OrderRepository(_context);
        public IPaymentRepository Payments => _payments ??= new PaymentRepository(_context);
        public  IOfferCodeRepository OfferCodes => _offerCodes ??= new OfferCodeRepository(_context);
        
        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
        public void Dispose() => _context.Dispose();
    }
}