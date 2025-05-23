using Application.Models.DTOs.Worker;

namespace Application.Services
{
    public interface IWorkerService
    {
        Task<ProfileDTO?> GetWorkerProfile(string email);
    }
}
