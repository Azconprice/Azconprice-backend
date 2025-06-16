using Application.Models.DTOs;
using Application.Models.DTOs.Pagination;
using Application.Models.DTOs.User;
using Application.Models.DTOs.Worker;
using Application.Repositories;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Supabase.Gotrue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User = Domain.Entities.User;

namespace Infrastructure.Services
{
    public class ClientService(UserManager<User> userManager, IMapper mapper, IBucketService bucketService, IMailService mailService, ICompanyProfileRepository companyProfileRepository, IWorkerProfileRepository workerProfileRepository) : IClientService
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly IMapper _mapper = mapper;
        private readonly IBucketService _bucketService = bucketService;
        private readonly IMailService _mailService = mailService;
        private readonly IWorkerProfileRepository _workerProfileRepository = workerProfileRepository;
        private readonly ICompanyProfileRepository _companyProfileRepository = companyProfileRepository;

        public async Task<bool> DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return false;

            await _userManager.DeleteAsync(user);
            return true;
        }

        public async Task<UserShowDTO?> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return null;

            var dto = _mapper.Map<UserShowDTO>(user);

            if (user.ProfilePicture != null)
            {
                dto.ProfilePicture = await _bucketService.GetSignedUrlAsync(user.ProfilePicture);
            }

            return dto;
        }

        public async Task<PaginatedResult<UserShowDTO>> GetAllUsersAsync(PaginationRequest request)
        {
            var workerUserIds = await _workerProfileRepository.Query()
              .Select(wp => wp.UserId)
              .ToListAsync();

            var companyUserIds = await _companyProfileRepository.Query()
                .Select(cp => cp.UserId)
                .ToListAsync();

            var excludedUserIds = workerUserIds.Concat(companyUserIds).ToHashSet();

            var usersQuery = _userManager.Users
                .AsNoTracking()
                .Where(u => !excludedUserIds.Contains(u.Id))
                .OrderByDescending(u => u.CreatedTime);

            var totalCount = await usersQuery.CountAsync();
            var users = await usersQuery
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var dtos = _mapper.Map<IEnumerable<UserShowDTO>>(users);

            foreach (var dto in dtos)
            {
                if (!string.IsNullOrEmpty(dto.ProfilePicture))
                {
                    dto.ProfilePicture = await _bucketService.GetSignedUrlAsync(dto.ProfilePicture);
                }
            }

            return new PaginatedResult<UserShowDTO>
            {
                Items = dtos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }

        public async Task<UserShowDTO?> UpdateUserAsync(string id, UserUpdateDTO model, Func<string, string, string> generateConfirmationUrl)
        {
            // Update related User entity
            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return null;

            bool emailChanged = false;

            if (!string.IsNullOrEmpty(model.FirstName))
                user.FirstName = model.FirstName;

            if (!string.IsNullOrEmpty(model.LastName))
                user.LastName = model.LastName;

            if (!string.IsNullOrEmpty(model.PhoneNumber))
                user.PhoneNumber = model.PhoneNumber;

            if (!string.IsNullOrEmpty(model.Email) && model.Email != user.Email)
            {
                var existingUser = _userManager.FindByEmailAsync(model.Email);
                if (existingUser is not null)
                    throw new InvalidOperationException("User with this email already exists");
                user.Email = model.Email;
                user.UserName = model.Email;
                emailChanged = true;
            }

            if (emailChanged)
            {
                var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                _mailService.SendConfirmationMessage(user.Email, confirmToken);
            }

            if (model.ProfilePicture != null && model.ProfilePicture.Length > 0)
            {
                string fileName;
                if (!string.IsNullOrEmpty(user.ProfilePicture))
                {
                    fileName = Path.GetFileName(user.ProfilePicture);
                }
                else
                {
                    fileName = $"profile/{Guid.NewGuid()}{Path.GetExtension(model.ProfilePicture.FileName)}";
                }

                var profilePictureUrl = await _bucketService.UploadAsync(model.ProfilePicture, fileName);
                user.ProfilePicture = profilePictureUrl;
            }

            await _userManager.UpdateAsync(user);
            return _mapper.Map<UserShowDTO>(user);
        }
    }
}