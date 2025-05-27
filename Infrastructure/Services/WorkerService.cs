using Application.Models.DTOs.Worker;
using Application.Repositories;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services
{
    public class WorkerService(UserManager<User> userManager, IWorkerProfileRepository workerProfileRepository, IProfessionRepository professionRepository) : IWorkerService
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly IWorkerProfileRepository _workerProfileRepository = workerProfileRepository;
        private readonly IProfessionRepository _professionRepository = professionRepository;

        public async Task<bool> DeleteWorkerProfile(string userId)
        {
            // Get the worker profile
            var workerProfile = await _workerProfileRepository.GetByUserIdAsync(userId);
            if (workerProfile == null)
                return false;

            // Get the related user
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            // Remove the worker profile
            _workerProfileRepository.Remove(workerProfile);
            await _workerProfileRepository.SaveChangesAsync();

            // Remove the user
            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<WorkerProfile?> GetWorkerProfile(string id)
        {
            try
            {
                var workerProfile = await _workerProfileRepository.GetByUserIdAsync(id);
                if (workerProfile is not null)
                {
                    return workerProfile;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Task<bool> UpdateWorkerProfile(WorkerUpdateProfileDTO model, string id)
        {
            throw new NotImplementedException();
        }
    }
}
