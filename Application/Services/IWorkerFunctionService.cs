using Application.Models.DTOs.Pagination;
using Application.Models.DTOs.WorkerFunction;
using Domain.Entities;

namespace Application.Services
{
    public interface IWorkerFunctionService
    {
        Task<bool> AddWorkerFunctionAsync(string userId, CreateWorkerFunctionRequest request);
        Task<WorkerFunctionShowDTO> UpdateWorkerFunctionAsync(string userId, string id, CreateWorkerFunctionRequest request);
        Task<bool> DeleteWorkerFunctionAsync(string userId, string id);
        Task<PaginatedResult<WorkerFunctionShowDTO>> GetAllWorkerFunctionsAsync(PaginationRequest request);
        Task<IEnumerable<WorkerFunctionShowDTO>> GetWorkerFunctionsByWorkerProfileIdAsync(string workerProfileId);
        Task<WorkerFunctionShowDTO?> GetWorkerFunctionByIdAsync(string id);
        Task<WorkerFunctionShowDTO> AddSpecialization(string userId, string workerFunctionId, string specializationId);
    }
}
