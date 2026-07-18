using Application.Interfaces;
using Infastructure.Repositories;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IProductRepository? _products;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IProductRepository Products => _products ??= new ProductRepository(_context);

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}