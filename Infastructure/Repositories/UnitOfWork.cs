using Application.Interfaces;
using Domain.Entities;
using Infastructure.Repositories;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IProductRepository? _products;
        IBrandRepository? _brands;
        ICategoryRepository _categories;
        IProductImageRepository _productImages;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IProductRepository Products => _products ??= new ProductRepository(_context);
        public IBrandRepository Brands => _brands ??= new BrandRepository(_context);
        public ICategoryRepository Categories =>_categories ??= new CategoryRepository(_context);
        public IProductImageRepository ProductImages => _productImages ??= new ProductImageRepository(_context);

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}