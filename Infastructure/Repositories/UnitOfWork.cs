using Application.Interfaces;
using Infrastructure.Repositories;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IProductRepository? _products;
        private IBrandRepository _brands;
        private ICategoryRepository _categories;
        private IProductImageRepository _productImages;
        private IUserRepository _userRepository;
        private IOtpVerificationRepository? _otpVerifications;
        private IRefreshTokenRepository? _refreshTokens;
        private IReviewRepository? _reviews;
        private ICartRepository? _carts;
        private ICartItemRepository? _cartItems;
        private IOrderRepository? _orders;
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IProductRepository Products => _products ??= new ProductRepository(_context);
        public IBrandRepository Brands => _brands ??= new BrandRepository(_context);
        public ICategoryRepository Categories =>_categories ??= new CategoryRepository(_context);
        public IProductImageRepository ProductImages => _productImages ??= new ProductImageRepository(_context);
        public IUserRepository Users => _userRepository ??= new UserRepository(_context);
        public IOtpVerificationRepository OtpVerifications =>
            _otpVerifications ??= new OtpVerificationRepository(_context);
        public IRefreshTokenRepository RefreshTokens =>
            _refreshTokens ??= new RefreshTokenRepository(_context);
        
        public IReviewRepository Reviews =>
            _reviews ??= new ReviewRepository(_context);
        
        public ICartRepository Carts => _carts ??= new CartRepository(_context);

        public ICartItemRepository CartItems =>  _cartItems ??= new CartItemRepository(_context);
        
        public IOrderRepository Orders => _orders ??= new OrderRepository(_context);

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
        public void Dispose() => _context.Dispose();
    }
}