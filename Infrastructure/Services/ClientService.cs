using Application.Models.DTOs;
using Application.Models.DTOs.User;
using Application.Models.DTOs.Worker;
using Application.Repositories;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Supabase.Gotrue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User = Domain.Entities.User;

namespace Infrastructure.Services
{
    public class ClientService(UserManager<User> userManager, IMapper mapper, IBucketService bucketService, IMailService mailService) : IClientService
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly IMapper _mapper = mapper;
        private readonly IBucketService _bucketService = bucketService;
        private readonly IMailService _mailService = mailService;

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

        public async Task<bool> UpdateUserAsync(string id, UserUpdateDTO model, Func<string, string, string> generateConfirmationUrl)
        {
            // Update related User entity
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return false;

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
            return true;
        }
    }
}