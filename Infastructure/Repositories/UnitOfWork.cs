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

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}