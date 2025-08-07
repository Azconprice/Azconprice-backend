using Domain.Entities;

namespace Application.Services
{
    public interface IWorkerFunctionSpecializationService
    {
        Task<bool> AddWorkerFunctionSpecializationAsync(Guid workerFunctionId, Guid specializationId);
        Task<bool> DeleteWorkerFunctionSpecializationAsync(Guid id);
        Task<bool> AddRangeOfSpecializationsToWorkerFunctionAsync(Guid workerFunctionId, IEnumerable<string> specializationIds);
        Task<IEnumerable<WorkerFunctionSpecialization>> GetWorkerFunctionSpecializationsByWorkerFunctionIdAsync(Guid workerFunctionId);

    }
}
