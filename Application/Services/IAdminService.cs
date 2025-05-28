using Application.Models.DTOs;
using Application.Models.DTOs.Log;
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
