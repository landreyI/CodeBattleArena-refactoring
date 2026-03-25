
namespace CodeBattleArena.Application.Common.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, CancellationToken ct);
        Task DeleteFileAsync(string fileUrl, CancellationToken ct);
    }
}
