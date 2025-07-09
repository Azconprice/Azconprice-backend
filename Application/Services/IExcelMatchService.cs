using Microsoft.AspNetCore.Http;

namespace Application.Services
{
    public interface IExcelMatchService
    {
        Task<(byte[] Content, string FileName, string ContentType)>
            ProcessQueryExcelAsync(IFormFile queryFile);
    }
}
