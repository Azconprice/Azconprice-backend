using Application.Models.DTOs;
using Application.Models.DTOs.Pagination;
using Application.Models.DTOs.Worker;

namespace Application.Services
{
    public interface IWorkerService
    {
        Task<WorkerProfileDTO?> GetWorkerProfile(string email);
        Task<WorkerProfileDTO?> UpdateWorkerProfile( string id, WorkerUpdateProfileDTO model, Func<string, string, string> generateConfirmationUrl);
        Task<bool> DeleteWorkerProfile(string id);
        Task<bool> AreSpecializationsValid(IEnumerable<string> specializationIds);
        Task<bool> ChangeWorkerPasswordAsync(string id, ChangePasswordDTO model);
        Task<PaginatedResult<WorkerProfileDTO>> GetAllWorkersAsync(PaginationRequest request);
    }
}
