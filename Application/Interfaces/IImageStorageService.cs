namespace Application.Interfaces
{
    public interface IImageStorageService
    {
        Task<string> UploadAsync(Stream fileStream, string fileName, string folder);
        Task DeleteAsync(string fileUrl);
    }
}
