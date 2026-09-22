using Amazon.S3;
using Amazon.S3.Model;
using Application.Interfaces;
using Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services
{
    public class S3FileStorageService : IImageStorageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly S3Settings _s3Settings;

        public S3FileStorageService(IAmazonS3 s3Client, IOptions<S3Settings> options)
        {
            _s3Client = s3Client;
            _s3Settings = options.Value;
        }

        public async Task<string> UploadAsync(Stream fileStream, string folder, string fileName, string contentType)
        {
            string objectKey = $"{folder}/{fileName}";
            var putRequest = new PutObjectRequest
            { 
                BucketName = _s3Settings.BucketName,
                Key = objectKey,
                InputStream = fileStream,
                ContentType = contentType,
                CannedACL = S3CannedACL.PublicRead
            };
            await _s3Client.PutObjectAsync(putRequest);

            return $"{_s3Settings.ServiceUrl}/{_s3Settings.BucketName}/{objectKey}";
        }

        public async Task DeleteAsync(string fileUrl)
        {
            var uri = new Uri(fileUrl);

            var key = uri.AbsolutePath.TrimStart('/')[(_s3Settings.BucketName.Length + 1)..];

            await _s3Client.DeleteObjectAsync(new DeleteObjectRequest
            {
                BucketName = _s3Settings.BucketName,
                Key = key
            });
        }
    }
}
