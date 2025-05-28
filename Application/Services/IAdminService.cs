using Application.Models.DTOs;
using Application.Models.DTOs.AppLogs;
using Application.Models.DTOs.Pagination;

namespace Application.Services
{
    public interface IAdminService
    {
        Task<bool> AddNewAdmin(AddAdminDTO model);
        Task<bool> ChangeCompanyStatus(string id);
        Task<PaginatedResult<LogListItemDTO>> GetLogsAsync(PaginationRequest request);
    }
}
