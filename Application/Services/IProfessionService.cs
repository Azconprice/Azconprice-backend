using Application.Models.DTOs.Category;

namespace Application.Services
{
    internal interface IProfessionService
    {
        Task<bool> AddProfessionAsync(string name, string description, string imageUrl);
        Task<bool> UpdateProfessionAsync(string id, string name, string description, string imageUrl);
        Task<bool> DeleteProfessionAsync(string id);
        Task<IEnumerable<ProfessionShowDTO>> GetAllProfessionsAsync();
        Task<ProfessionShowDTO?> GetProfessionByIdAsync(string id);
    }
}
