namespace Application.Interfaces
{
    public interface IImageStorageService
    {
        Task<string> UploadAsync(Stream fileStream, string folder, string fileName, string contentType);
        Task DeleteAsync(string fileUrl);
    }
}
