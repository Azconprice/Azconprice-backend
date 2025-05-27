using Application.Models.DTOs.Profession;

namespace Application.Services
{
    public interface IProfessionService
    {
        Task<bool> AddProfessionAsync(ProfessionDTO profession);
        Task<bool> UpdateProfessionAsync(string id, ProfessionUpdateDTO updatedProfession);
        Task<bool> DeleteProfessionAsync(string id);
        Task<IEnumerable<ProfessionShowDTO>> GetAllProfessionsAsync();
        Task<ProfessionShowDTO?> GetProfessionByIdAsync(string id);
    }
}
