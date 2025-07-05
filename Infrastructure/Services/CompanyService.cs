using Application.Models.DTOs;
using Application.Models.DTOs.Company;
using Application.Models.DTOs.Pagination;
using Application.Models.DTOs.User;
using Application.Models.DTOs.Worker;
using Application.Repositories;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class CompanyService(
        ICompanyProfileRepository companyProfileRepository,
        ISalesCategoryRepository salesCategoryRepository,
        UserManager<User> userManager,
        IMapper mapper,
        IBucketService bucketService) : ICompanyService
    {
        private readonly ICompanyProfileRepository _companyProfileRepository = companyProfileRepository;
        private readonly ISalesCategoryRepository _salesCategoryRepository = salesCategoryRepository;
        private readonly IBucketService _bucketService = bucketService;
        private readonly UserManager<User> _userManager = userManager;
        private readonly IMapper _mapper = mapper;

        public async Task<bool> DeleteCompanyProfile(string id)
        {
            var companyProfile = await _companyProfileRepository.GetAsync(c => c.UserId == id);
            if (companyProfile == null)
                return false;

            _companyProfileRepository.Remove(companyProfile);
            await _companyProfileRepository.SaveChangesAsync();
            return true;
        }

        public async Task<PaginatedResult<CompanyProfileDTO>> GetAllCompaniesAsync(PaginationRequest request)
        {
            var query = _companyProfileRepository.Query().OrderByDescending(r => r.CreatedTime);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PaginatedResult<CompanyProfileDTO>
            {
                Items = _mapper.Map<IEnumerable<CompanyProfileDTO>>(items),
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };

        }

        public async Task<CompanyProfileDTO?> GetCompanyProfile(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return null;

            var companyProfile = await _companyProfileRepository.GetByUserIdAsync(user.Id);
            if (companyProfile == null)
                return null;

            // Map CompanyProfile to WorkerProfileDTO for example purposes
            var dto = _mapper.Map<CompanyProfileDTO>(companyProfile);
            var url = await _bucketService.GetSignedUrlAsync(companyProfile.TaxId);

            dto.TaxId = url;
            return dto;
        }

        public async Task<bool> IsSalesCategoryValid(string salesCategoryId)
        {
            var category = await _salesCategoryRepository.GetAsync(salesCategoryId);
            return category != null;
        }

        public async Task<CompanyProfileDTO?> UpdateCompanyProfile(string id, UpdateCompanyProfileDTO model, Func<string, string, string> generateConfirmationUrl)
        {
            var companyProfile = await _companyProfileRepository.GetByUserIdAsync(id);
            if (companyProfile == null)
                return null;

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return null;

            bool emailChanged = false;

            if (!string.IsNullOrEmpty(model.Email) && model.Email != user.Email)
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser is not null)
                    throw new InvalidOperationException("User with this email already exists");
                user.Email = model.Email;
                user.UserName = model.Email;
                emailChanged = true;
            }

            if (!string.IsNullOrEmpty(model.PhoneNumber))
                user.PhoneNumber = model.PhoneNumber.Replace(" ", "").Replace("-", "");

            await _userManager.UpdateAsync(user);

            if (emailChanged)
            {
                var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationUrl = generateConfirmationUrl(user.Id, confirmToken);
            }

            if (!string.IsNullOrEmpty(model.Address))
                companyProfile.User.Address = model.Address;

            if (!string.IsNullOrEmpty(model.SalesCategoryId) && await IsSalesCategoryValid(model.SalesCategoryId) && Guid.TryParse(model.SalesCategoryId, out var salesCategoryId))
                companyProfile.SalesCategoryId = salesCategoryId;

            if (model.Logo != null && model.Logo.Length > 0)
            {
                string fileName;
                if (!string.IsNullOrEmpty(companyProfile.CompanyLogo))
                {
                    fileName = System.IO.Path.GetFileName(companyProfile.CompanyLogo);
                }
                else
                {
                    fileName = $"profile/{Guid.NewGuid()}{System.IO.Path.GetExtension(model.Logo.FileName)}";
                }

                var profilePictureUrl = await _bucketService.UploadAsync(model.Logo, fileName);
                companyProfile.CompanyLogo = profilePictureUrl;
            }

            _companyProfileRepository.Update(companyProfile);
            await _companyProfileRepository.SaveChangesAsync();

            // Map CompanyProfile to WorkerProfileDTO for example purposes
            var dto = _mapper.Map<CompanyProfileDTO>(companyProfile);

            return dto;
        }
        public async Task<bool> ChangeCompanyPasswordAsync(string id, ChangePasswordDTO model)
        {
            var user = await _userManager.FindByIdAsync(id) ?? throw new InvalidOperationException("Company not found.");

            if (await _userManager.CheckPasswordAsync(user, model.OldPassword) is not true)
                throw new InvalidOperationException("Old password is incorrect.");

            await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            return true;
        }
    }
}
