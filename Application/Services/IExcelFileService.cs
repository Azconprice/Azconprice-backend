using Application.Models.DTOs.Excel;
using Application.Models.DTOs.Pagination;
using Microsoft.AspNetCore.Http;

namespace Application.Services
{
    public interface IExcelFileService
    {
        Task<PaginatedResult<ExcelFileDTO>> GetExcelFilesAsync(PaginationRequest request);
        Task<int> GetExcelFileCountAsync();
        Task<ExcelFileDTO> UploadExcelAsync(IFormFile file, string firstName, string lastName, string email, string userId);
    }
}
