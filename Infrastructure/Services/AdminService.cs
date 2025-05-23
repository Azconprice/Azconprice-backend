using Application.Models.DTOs;
using Application.Models.DTOs.Category;
using Application.Repositories;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services
{
    public class AdminService : IAdminService
    {
        private readonly IProfessionRepository _professionRepository;
        private readonly UserManager<User> _userManager;

        public AdminService(IProfessionRepository professionRepository, UserManager<User> userManager)
        {
            _professionRepository = professionRepository;
            _userManager = userManager;
        }

        public async Task<bool> AddProfessionAsync(ProfessionDTO model)
        {
            try
            {
                var category = new Profession { Name = model.Name, Description = model.Description };
                await _professionRepository.AddAsync(category);
                await _professionRepository.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

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
            catch (Exception)
            {
                return false;
            }
        }

        public IEnumerable<ProfessionShowDTO> GetAllProfessions() => _professionRepository.GetAll().Select(c => new ProfessionShowDTO { Id = c.Id.ToString(), Name = c.Name });

        public async Task<bool> UpdateProfessionAsync(ProfessionUpdateDTO model,string id)
        {
            try
            {
                var profession = await _professionRepository.GetAsync(id);

                if (profession == null)
                    return false;

                profession.Name = model.Name;
                profession.Description = model.Description;

                _professionRepository.Update(profession);
                await _professionRepository.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
