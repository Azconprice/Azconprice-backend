using Application.Models.DTOs;
using Application.Models.DTOs.AppLogs;
using Application.Models.DTOs.Pagination;
using Application.Models.DTOs.Profession;
using Application.Repositories;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class AdminService(UserManager<User> userManager, 
        ICompanyProfileRepository companyProfileRepository, 
        IAppLogRepository logRepository,
        IExcelFileRecordRepository excelFileRecordRepository,
        IRequestRepository requestRepository) : IAdminService
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly ICompanyProfileRepository _companyProfileRepository = companyProfileRepository;
        private readonly IAppLogRepository _logRepository = logRepository;
        private readonly IExcelFileRecordRepository _excelFileRecordRepository = excelFileRecordRepository;
        private readonly IRequestRepository _requestRepository = requestRepository;

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

        public async Task<DashboardStatistics> GetDashboardStatisticsAsync(DateIntervalRequest dateInterval)
        {
            var start = DateTime.SpecifyKind(dateInterval.StartDate.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
            var end = DateTime.SpecifyKind(dateInterval.EndDate.ToDateTime(TimeOnly.MaxValue), DateTimeKind.Utc);

            var userCount = await _userManager.Users
                .Where(u => u.CreatedTime >= start && u.CreatedTime <= end)
                .CountAsync();

            var excelFilesCount = await _excelFileRecordRepository
                .Query()
                .Where(f => f.UploadedAt >= start && f.UploadedAt <= end)
                .CountAsync();

            var requestsCount = await _requestRepository
                .Query()
                .Where(r => r.CreatedTime >= start && r.CreatedTime <= end)
                .CountAsync();

            return new DashboardStatistics
            {
                UserCount = userCount,
                UploadedFilesCount = excelFilesCount,
                RequestsCount = requestsCount,
                TotalMoneyAmount = 0
            };
        }

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
