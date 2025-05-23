using Application.Models.DTOs.Worker;
using Application.Repositories;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services
{
    public class WorkerService(UserManager<User> userManager, IMailService mailService, IProfessionRepository professionRepository) : IWorkerService
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly IProfessionRepository _professionRepository = professionRepository;

        public async Task<ProfileDTO?> GetWorkerProfile(string email)
        {
            try
            {
                var worker = await _userManager.FindByEmailAsync(email);
                if (worker is not null)
                {
                    var dto = new ProfileDTO
                    {
                        FirstName = worker.FirstName,
                        LastName = worker.LastName,
                        Email = worker.Email,
                    };
                    return dto;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}
