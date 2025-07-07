using Microsoft.AspNetCore.Http;

namespace Application.Models.DTOs.Excel
{
    public class ExcelUploadRequest
    {
        public IFormFile File { get; set; } = default!;
        public bool IsSimple { get; set; }
    }
}
