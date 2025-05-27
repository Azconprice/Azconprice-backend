using Application.Models.DTOs.Specialization;
using Application.Repositories;
using Application.Services;
using Domain.Entities;

namespace Infrastructure.Services
{
    public class SpecializationService : ISpecializationService
    {
        private readonly ISpecializationRepository _specializationRepository;

        public SpecializationService(ISpecializationRepository specializationRepository)
        {
            _specializationRepository = specializationRepository;
        }

        public async Task<bool> AddSpecializationAsync(SpecializationDTO specialization)
        {
            var exists = _specializationRepository
                .GetWhere(s => s.ProfessionId == specialization.ProfessionId &&
                               s.Name.ToLower() == specialization.Name.ToLower())
                .Any();

            if (exists)
                throw new InvalidOperationException("A specialization with this name already exists for the selected profession.");

            var newSpecialization = new Specialization
            {
                CreatedTime = DateTime.UtcNow,
                ProfessionId = specialization.ProfessionId,
                Name = specialization.Name,
            };

            await _specializationRepository.AddAsync(newSpecialization);
            await _specializationRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteSpecializationAsync(string id)
        {
            var deleted = await _specializationRepository.RemoveAsync(id);
            if (!deleted)
                throw new InvalidOperationException("Specialization not found or could not be deleted.");
            await _specializationRepository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<SpecializationShowDTO>> GetAllSpecializationsAsync()
        {
            var specializations = await _specializationRepository.GetAllAsync(false);
            return specializations.Select(s => new SpecializationShowDTO
            {
                Id = s.Id,
                Name = s.Name,
                ProfessionId = s.ProfessionId,
            });
        }

        public async Task<SpecializationShowDTO?> GetSpecializationByIdAsync(string id)
        {
            var specialization = await _specializationRepository.GetAsync(id);
            if (specialization == null)
                return null;

            return new SpecializationShowDTO
            {
                Id = specialization.Id,
                Name = specialization.Name,
                ProfessionId = specialization.ProfessionId,
            };
        }

        public async Task<bool> UpdateSpecializationAsync(string id, SpecializationUpdateDTO updatedSpecialization)
        {
            var specialization = await _specializationRepository.GetAsync(id) ?? throw new InvalidOperationException("Specialization not found.");

            // Check for name uniqueness (case-insensitive) within the same profession
            var exists = _specializationRepository
                .GetWhere(s => s.ProfessionId == specialization.ProfessionId &&
                               s.Name.ToLower() == updatedSpecialization.Name.ToLower() &&
                               s.Id != specialization.Id)
                .Any();
            if (exists)
                throw new InvalidOperationException("A specialization with this name already exists for the selected profession.");

            specialization.Name = updatedSpecialization.Name;
            specialization.ProfessionId = updatedSpecialization.ProfessionId;

            _specializationRepository.Update(specialization);
            await _specializationRepository.SaveChangesAsync();
            return true;
        }
    }
}
