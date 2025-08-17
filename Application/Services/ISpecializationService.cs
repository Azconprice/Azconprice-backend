using Application.Models.DTOs.Specialization;

namespace Application.Services
{
    public interface ISpecializationService
    {
        Task<bool> AddSpecializationAsync(SpecializationDTO specialization);
        Task<bool> UpdateSpecializationAsync(string id, SpecializationUpdateDTO updatedSpecialization);
        Task<bool> DeleteSpecializationAsync(string id);
        Task<IEnumerable<SpecializationShowDTO>> GetAllSpecializationsAsync();
        Task<SpecializationShowDTO?> GetSpecializationByIdAsync(string id);
        Task<IEnumerable<SpecializationShowDTO>> SearchSpecializationAsync(string query);
        Task<IEnumerable<SpecializationShowDTO>> SearchSpecializationByProfessionAsync(string query, string professionId);
        Task<IEnumerable<SpecializationShowDTO>> FilterByProfessionAsync(string professionId);
    }
}
