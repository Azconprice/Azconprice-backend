using Microsoft.AspNetCore.Http;

namespace Application.Services
{
    public interface IBucketService
    {
        Task<string> UploadAsync(IFormFile file);
        Task<string> UploadAsync(IFormFile file,string fileName);
        Task<string> GetSignedUrlAsync(string path, int expiresInSeconds = 3600);
    }
}
