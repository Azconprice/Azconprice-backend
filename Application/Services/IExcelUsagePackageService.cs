using Application.Models;
using Domain.Entities;

namespace Application.Services
{
    public interface IExcelUsagePackageService
    {
        Task<IEnumerable<ExcelUsagePackageShowDTO>> GetExcelUsagePackagesByUserIdAsync(string userId);
        Task<ExcelUsagePackageShowDTO> GetExcelUsagePackageByIdAsync(Guid id);
    }
}
