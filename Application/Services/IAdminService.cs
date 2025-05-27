using Application.Models.DTOs;
using Application.Models.DTOs.Profession;

namespace Application.Services
{
    public interface IAdminService
    {
        public Task<bool> AddProfessionAsync(ProfessionDTO model);
        public Task<bool> UpdateProfessionAsync(ProfessionUpdateDTO model,string id);
        public Task<bool> AddNewAdmin(AddAdminDTO model);
        public IEnumerable<ProfessionShowDTO> GetAllProfessions();
    }
}
