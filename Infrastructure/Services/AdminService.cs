using Application.Models.DTOs;
using Application.Models.DTOs.Profession;
using Application.Repositories;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services
{
    public class AdminService(UserManager<User> userManager, ICompanyProfileRepository companyProfileRepository) : IAdminService
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly ICompanyProfileRepository _companyProfileRepository = companyProfileRepository;

        public async Task<bool> AddNewAdmin(AddAdminDTO model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user is null)
                {
                    user = new User
                    {
                        FirstName = "admin",
                        LastName = "admin",
                        UserName = model.Email,
                        Email = model.Email,
                        EmailConfirmed = true
                    };
                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (!result.Succeeded)
                        return false;

                    result = await _userManager.AddToRoleAsync(user, "Admin");
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while adding a new admin.", ex);
            }
        }

        public async Task<bool> ChangeCompanyStatus(string id)
        {
            var company = await _companyProfileRepository.GetAsync(id);
            if (company is null)
                return false;

            company.IsConfirmed = !company.IsConfirmed;
            _companyProfileRepository.Update(company);
            await _companyProfileRepository.SaveChangesAsync();
            return true;
        }
    }
}
