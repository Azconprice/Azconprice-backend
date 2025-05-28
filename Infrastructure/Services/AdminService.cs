using Application.Models.DTOs;
using Application.Models.DTOs.Log;
using Application.Models.DTOs.Pagination;
using Application.Models.DTOs.Profession;
using Application.Repositories;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services
{
    public class AdminService(UserManager<User> userManager, ICompanyProfileRepository companyProfileRepository,IAppLogRepository logRepository) : IAdminService
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly ICompanyProfileRepository _companyProfileRepository = companyProfileRepository;
        private readonly IAppLogRepository _logRepository = logRepository;

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

        // Fix for CS1061: 'IAppLogRepository' does not contain a definition for 'Query'.
        // The error indicates that the 'Query' method is not defined in the IAppLogRepository interface.
        // Based on the IRepository<T> interface provided, we can use GetWhere or GetAll methods to retrieve logs.

        public async Task<PaginatedResult<LogListItemDTO>> GetLogsAsync(PaginationRequest request)
        {
            // Fetch all logs and order them by Timestamp descending
            var logs = await _logRepository.GetAllAsync();
            var query = logs.OrderByDescending(l => l.Timestamp);

            var totalCount = query.Count();
            var items = query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(l => new LogListItemDTO
                {
                    Id = l.Id,
                    Action = l.Action,
                    RelatedEntityId = l.RelatedEntityId,
                    UserName = l.UserName,
                    Timestamp = l.Timestamp,
                    Details = l.Details
                })
                .ToList();

            return new PaginatedResult<LogListItemDTO>
            {
                Items = items,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}
