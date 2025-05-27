using Application.Models.DTOs.Worker;

namespace Application.Services
{
    public interface IWorkerService
    {
        Task<WorkerProfileDTO?> GetWorkerProfile(string email);
        Task<bool> UpdateWorkerProfile(WorkerUpdateProfileDTO model, string id);
        Task<bool> DeleteWorkerProfile(string id);
        Task<bool> AreSpecializationsValid(IEnumerable<string> specializationIds);
    }
}
