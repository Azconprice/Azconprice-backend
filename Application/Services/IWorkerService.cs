using Application.Models.DTOs.Worker;
using Domain.Entities;

namespace Application.Services
{
    public interface IWorkerService
    {
        Task<WorkerProfile?> GetWorkerProfile(string email);
        Task<bool> UpdateWorkerProfile(WorkerUpdateProfileDTO model, string id);
        Task<bool> DeleteWorkerProfile(string id);

    }
}
