using Application.Models.DTOs.Pagination;
using Application.Models.DTOs.WorkerFunction;
using Domain.Entities;

namespace Application.Services
{
    public interface IWorkerFunctionService
    {
        Task<bool> AddWorkerFunctionAsync(CreateWorkerFunctionRequest request);
        Task<WorkerFunction> UpdateWorkerFunctionAsync(Guid id, CreateWorkerFunctionRequest request);
        Task<bool> DeleteWorkerFunctionAsync(Guid id);
        Task<PaginatedResult<WorkerFunctionShowDTO>> GetAllWorkerFunctionsAsync(PaginationRequest request);
        Task<IEnumerable<WorkerFunctionShowDTO>> GetWorkerFunctionsByWorkerProfileIdAsync(Guid workerProfileId);
        Task<WorkerFunctionShowDTO?> GetWorkerFunctionByIdAsync(Guid id);
    }
}
