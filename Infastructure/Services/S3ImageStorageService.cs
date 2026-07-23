using Amazon.S3;
using Amazon.S3.Model;
using Application.Interfaces;
using Infastructure.Settings;
using Microsoft.Extensions.Options;

namespace Infastructure.Services
{
    public class S3ImageStorageService : IImageStorageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly S3Settings _s3Settings;
        private static class Folders
        {
            public const string CategoryImage = "categories/{0}/image/";
            public const string BrandImage = "brands/{0}/image/";
            public const string ProductImages = "products/{0}/images/";
            public const string UserProfileImage = "users/{0}/ProfileImage/";
        }

        public S3ImageStorageService(IAmazonS3 s3Client, IOptions<S3Settings> options)
        {
            _s3Client = s3Client;
            _s3Settings = options.Value;
        }

        public async Task<string> UploadAsync(Stream fileStream, string folder, string fileName)
        {

            string objectKey = $"{folder}{fileName}";

            var putRequest = new PutObjectRequest
            {
                BucketName = _s3Settings.BucketName,
                Key = objectKey,
                InputStream = fileStream,
                CannedACL = S3CannedACL.PublicRead
            };
            await _s3Client.PutObjectAsync(putRequest);

            return $"{_s3Settings.ServiceURL}/{_s3Settings.BucketName}/{objectKey}";
        }

        public Task DeleteAsync(string fileUrl)
        {
            throw new NotImplementedException();
        }

        //public Task<string> SaveUserProfileImageAsync(Stream fileStream, int userId)
        //    => UploadAsync(fileStream, string.Format(Folders.UserProfileImage, userId));
    }
}
