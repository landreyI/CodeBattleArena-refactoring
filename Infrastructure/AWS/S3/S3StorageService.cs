

using Amazon.S3;
using Amazon.S3.Model;
using CodeBattleArena.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace CodeBattleArena.Infrastructure.AWS.S3
{
    public class S3StorageService : IFileStorageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public S3StorageService(IAmazonS3 s3Client, IConfiguration config)
        {
            _s3Client = s3Client;
            _bucketName = config["AWS:BucketName"] ?? throw new ArgumentException("BucketName is missing");
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, CancellationToken ct)
        {
            var fileKey = $"items/{Guid.NewGuid()}-{fileName}";

            var putRequest = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = fileKey,
                InputStream = fileStream,
                ContentType = contentType,
                // доступен для чтения по прямой ссылке
                CannedACL = S3CannedACL.PublicRead
            };

            await _s3Client.PutObjectAsync(putRequest, ct);

            return $"https://{_bucketName}.s3.amazonaws.com/{fileKey}";
        }

        public async Task DeleteFileAsync(string fileUrl, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(fileUrl)) return;

            // Вытаскиваем Key
            var uri = new Uri(fileUrl);
            var key = uri.AbsolutePath.TrimStart('/');

            await _s3Client.DeleteObjectAsync(_bucketName, key, ct);
        }
    }
}
