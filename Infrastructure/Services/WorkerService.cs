using Application.Models.DTOs.Worker;
using Application.Models.DTOs.User;
using Application.Repositories;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services
{
    public class WorkerService(
        IWorkerProfileRepository workerProfileRepository,
        IMapper mapper,
        IBucketService bucketService,
        IMailService mailService,
        ISpecializationRepository specializationsRepository,
        UserManager<User> userManager) : IWorkerService
    {
        private readonly IWorkerProfileRepository _workerProfileRepository = workerProfileRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IBucketService _bucketService = bucketService;
        private readonly IMailService _mailService = mailService;
        private readonly ISpecializationRepository _specializationsRepository = specializationsRepository;
        private readonly UserManager<User> _userManager = userManager;

        public async Task<bool> DeleteWorkerProfile(string userId)
        {
            var workerProfile = await _workerProfileRepository.GetByUserIdAsync(userId);
            if (workerProfile == null)
                return false;

            _workerProfileRepository.Remove(workerProfile);
            await _workerProfileRepository.SaveChangesAsync();

            return true;
        }

        public async Task<WorkerProfileDTO?> GetWorkerProfile(string id)
        {
            var workerProfile = await _workerProfileRepository.GetByUserIdAsync(id);
            if (workerProfile == null)
                return null;

            var dto = _mapper.Map<WorkerProfileDTO>(workerProfile);

            // Fetch signed URL for user's avatar if available
            if (workerProfile.User?.ProfilePicture != null)
            {
                dto.User.ProfilePicture = await _bucketService.GetSignedUrlAsync(workerProfile.User.ProfilePicture);
            }

            return dto;
        }

        public async Task<WorkerProfileDTO?> UpdateWorkerProfile(string id, WorkerUpdateProfileDTO model, Func<string, string, string> generateConfirmationUrl)
        {
            var workerProfile = await _workerProfileRepository.GetByUserIdAsync(id);
            if (workerProfile == null)
                return null;

            // Update related User entity
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return null;

            bool emailChanged = false;

            if (!string.IsNullOrEmpty(model.FirstName))
                user.FirstName = model.FirstName;

            if (!string.IsNullOrEmpty(model.LastName))
                user.LastName = model.LastName;

            if (!string.IsNullOrEmpty(model.PhoneNumber))
                user.PhoneNumber = model.PhoneNumber;

            if (!string.IsNullOrEmpty(model.Email) && model.Email != user.Email)
            {
                var existingUser = _userManager.FindByEmailAsync(model.Email);
                if (existingUser is not null)
                    throw new InvalidOperationException("Worker with this email already exists");
                user.Email = model.Email;
                user.UserName = model.Email;
                emailChanged = true;
            }

            await _userManager.UpdateAsync(user);

            // If email was changed, send confirmation email
            if (emailChanged)
            {
                var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                // You may need to provide a URL generator or pass the confirmation link format here
                // For now, just send the token (integration with controller or frontend needed for full link)
                _mailService.SendConfirmationMessage(user.Email, confirmToken);
            }

            // Specialization validation
            if (model.Specizalizations != null && model.Specizalizations.Any())
            {
                if (!await AreSpecializationsValid(model.Specizalizations))
                    throw new InvalidOperationException("One or more specialization IDs are invalid.");
            }

            // Update WorkerProfile fields
            if (model.HaveTaxId.HasValue)
                workerProfile.HaveTaxId = model.HaveTaxId.Value;

            if (model.TaxId != null)
                workerProfile.TaxId = model.TaxId;

            if (model.Address != null)
                workerProfile.Address = model.Address;

            if (model.Experience.HasValue)
                workerProfile.Experience = model.Experience.Value;

            if (model.Price.HasValue)
                workerProfile.Price = model.Price.Value;

            if (model.ProfilePicture != null && model.ProfilePicture.Length > 0)
            {
                string fileName;
                if (!string.IsNullOrEmpty(workerProfile.User.ProfilePicture))
                {
                    fileName = System.IO.Path.GetFileName(workerProfile.User.ProfilePicture);
                }
                else
                {
                    fileName = $"profile/{Guid.NewGuid()}{System.IO.Path.GetExtension(model.ProfilePicture.FileName)}";
                }

                var profilePictureUrl = await _bucketService.UploadAsync(model.ProfilePicture, fileName);
                workerProfile.User.ProfilePicture = profilePictureUrl;
            }

            var dto = _mapper.Map<WorkerProfileDTO>(workerProfile);

            _workerProfileRepository.Update(workerProfile);
            await _workerProfileRepository.SaveChangesAsync();
            return dto;
        }

        public async Task<bool> AreSpecializationsValid(IEnumerable<string> specializationIds)
        {
            if (specializationIds == null || !specializationIds.Any())
                return true;

            // Convert string IDs to Guid for comparison
            var guidIds = specializationIds
                .Select(id => Guid.TryParse(id, out var guid) ? guid : Guid.Empty)
                .Where(guid => guid != Guid.Empty)
                .ToList();

            if (guidIds.Count != specializationIds.Count())
                return false; // Some IDs were not valid GUIDs

            var allSpecializations = await _specializationsRepository.GetAllAsync(false);
            var validCount = allSpecializations.Count(s => guidIds.Contains(s.Id));

            return validCount == guidIds.Count;
        }
    }
}
