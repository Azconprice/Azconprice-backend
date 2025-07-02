using Application.Models.DTOs;
using Application.Models.DTOs.Company;
using Application.Models.DTOs.Pagination;
using Application.Models.DTOs.Worker;

namespace Application.Services
{
    public interface ICompanyService
    {
        Task<CompanyProfileDTO?> GetCompanyProfile(string email);
        Task<CompanyProfileDTO?> UpdateCompanyProfile(string id, UpdateCompanyProfileDTO model, Func<string, string, string> generateConfirmationUrl);
        Task<bool> DeleteCompanyProfile(string id);
        Task<bool> IsSalesCategoryValid(string salesCategoryId);
        Task<bool> ChangeCompanyPasswordAsync(string id, ChangePasswordDTO model);
        Task<PaginatedResult<CompanyProfileDTO>> GetAllCompaniesAsync(PaginationRequest request);
    }
}
