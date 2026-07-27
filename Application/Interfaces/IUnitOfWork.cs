namespace Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        IBrandRepository Brands { get; }
        ICategoryRepository Categories { get; }
        IProductImageRepository ProductImages { get; }
        Task<int> SaveChangesAsync();
    }
}