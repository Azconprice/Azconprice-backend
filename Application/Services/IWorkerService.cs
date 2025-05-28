using Application.Models.DTOs.Worker;

namespace Application.Services
{
    public interface IWorkerService
    {
        Task<WorkerProfileDTO?> GetWorkerProfile(string email);
        Task<WorkerProfileDTO?> UpdateWorkerProfile( string id, WorkerUpdateProfileDTO model, Func<string, string, string> generateConfirmationUrl);
        Task<bool> DeleteWorkerProfile(string id);
        Task<bool> AreSpecializationsValid(IEnumerable<string> specializationIds);
    }
}
