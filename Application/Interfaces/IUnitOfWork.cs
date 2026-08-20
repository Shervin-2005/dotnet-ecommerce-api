namespace Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        IBrandRepository Brands { get; }
        ICategoryRepository Categories { get; }
        IProductImageRepository ProductImages { get; }
        IUserRepository Users {  get; }
        IOtpVerificationRepository OtpVerifications { get; }
        IRefreshTokenRepository? RefreshTokens {  get; }
        Task<int> SaveChangesAsync();
    }
}