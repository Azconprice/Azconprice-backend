using Application.Models.DTOs.Company;
using Application.Models.DTOs.Worker;

namespace Application.Services
{
    public interface ICompanyService
    {
        Task<CompanyProfileDTO?> GetCompanyProfile(string email);
        Task<CompanyProfileDTO?> UpdateCompanyProfile(string id, UpdateCompanyProfileDTO model, Func<string, string, string> generateConfirmationUrl);
        Task<bool> DeleteCompanyProfile(string id);
        Task<bool> IsSalesCategoryValid(string salesCategoryId);
    }
}
