using Application.Models.DTOs.Excel;
using Application.Models.DTOs.Pagination;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Application.Services
{
    public interface IExcelFileService
    {
        Task<PaginatedResult<ExcelFileDTO>> GetExcelFilesAsync(PaginationRequest request);
        Task<int> GetExcelFileCountAsync();
        Task<ExcelFileDTO> UploadExcelAsync(IFormFile file, string firstName, string lastName, string email, string userId);
        Task<PaginatedResult<ExcelFileDTO>> GetExcelFilesByUserAsync(string userId, PaginationRequest request);
        public FileContentResult ProcessQueryExcelAsync(IFormFile queryFile, string? userId = null,bool isSimple = false);
    }
}
