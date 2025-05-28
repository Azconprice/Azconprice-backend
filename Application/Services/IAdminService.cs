using Application.Models.DTOs;
using Application.Models.DTOs.Profession;

namespace Application.Services
{
    public interface IAdminService
    {
        Task<bool> AddNewAdmin(AddAdminDTO model);
        Task<bool> ChangeCompanyStatus(string id);
    }
}
