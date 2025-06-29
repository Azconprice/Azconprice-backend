using Application.Models.DTOs;
using Application.Models.DTOs.Pagination;

namespace Application.Services
{
    public interface IRequestService
    {
        Task<PaginatedResult<RequestShowDTO>> GetAllRequestsAsync(PaginationRequest request);
        Task<PaginatedResult<RequestShowDTO>> GetUserRequestsAsync(PaginationRequest request, string mail);
        Task<RequestShowDTO?> GetRequestByIdAsync(string requestId);
        Task<RequestShowDTO?> CreateRequestAsync(RequestDTO request);
        Task<bool> DeleteRequestAsync(string requestId);
        Task<PaginatedResult<RequestShowDTO>> GetRequestByTypeAsync(PaginationRequest request, string type);
    }
}
