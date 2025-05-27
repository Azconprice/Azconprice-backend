using Application.Models.DTOs.Profession;
using Application.Repositories;
using Application.Services;
using Domain.Entities;

namespace Infrastructure.Services
{
    public class ProfessionService : IProfessionService
    {
        private readonly IProfessionRepository _professionRepository;

        public ProfessionService(IProfessionRepository professionRepository)
        {
            _professionRepository = professionRepository;
        }

        public async Task<bool> AddProfessionAsync(ProfessionDTO profession)
        {
            var exists = _professionRepository
                .GetWhere(p => p.Name.ToLower() == profession.Name.ToLower())
                .Any();

            if (exists)
                throw new InvalidOperationException("A profession with this name already exists.");

            var newProfession = new Profession
            {
                CreatedTime = DateTime.UtcNow,
                Name = profession.Name,
                Description = profession.Description,
            };

            await _professionRepository.AddAsync(newProfession);
            await _professionRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateProfessionAsync(string id, ProfessionUpdateDTO updatedProfession)
        {
            var profession = await _professionRepository.GetAsync(id) ?? throw new InvalidOperationException("Profession not found.");

            // Check for name uniqueness (case-insensitive), excluding current profession
            var exists = _professionRepository
                .GetWhere(p => p.Name.ToLower() == updatedProfession.Name.ToLower() && p.Id != profession.Id)
                .Any();
            if (exists)
                throw new InvalidOperationException("A profession with this name already exists.");

            profession.Name = updatedProfession.Name;
            profession.Description = updatedProfession.Description;

            _professionRepository.Update(profession);
            await _professionRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteProfessionAsync(string id)
        {
            var deleted = await _professionRepository.RemoveAsync(id);
            if (!deleted)
                throw new InvalidOperationException("Profession not found or could not be deleted.");
            await _professionRepository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ProfessionShowDTO>> GetAllProfessionsAsync()
        {
            var professions = await _professionRepository.GetAllAsync(false);
            return professions.Select(p => new ProfessionShowDTO
            {
                Id = p.Id.ToString(),
                Name = p.Name,
                Description = p.Description
            });
        }

        public async Task<ProfessionShowDTO?> GetProfessionByIdAsync(string id)
        {
            var profession = await _professionRepository.GetAsync(id);
            if (profession == null)
                return null;

            return new ProfessionShowDTO
            {
                Id = profession.Id.ToString(),
                Name = profession.Name,
                Description = profession.Description
            };
        }
    }
}
