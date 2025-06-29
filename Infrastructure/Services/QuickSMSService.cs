using Application.Models;
using Application.Repositories;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Twilio.Types;

namespace Infrastructure.Services
{
    public class QuickSMSService(QuickSMSOptions options, IAppLogger appLogger, IOtpVerificationRepository repository, UserManager<User> userManager) : ISMSService
    {
        private readonly QuickSMSOptions _options = options;
        private readonly IAppLogger _appLogger = appLogger;
        private readonly IOtpVerificationRepository _repository = repository;
        private readonly UserManager<User> _userManager = userManager;

        public async Task<string> SendVerificationCodeAsync(string phoneNumber)
        {
            HttpClient _httpClient = new();
            string _url = _options.PostUrl;
            var code = GenerateOtpCode();
            var message = $"Bu, hesabın təsdiqlənməsi üçün birdəfəlik OTP kodunuzdur: {code}";
            var formattedPhoneNumber = phoneNumber.Replace("+", "").Replace(" ", "").Replace("-", "");


            string passwordMd5 = CalculateMD5Hash(_options.Password);
            string keyRaw = passwordMd5 + _options.Username + message + formattedPhoneNumber + _options.Sender;
            string key = CalculateMD5Hash(keyRaw);

            var body = new
            {
                login = _options.Username,
                key,
                msisdn = formattedPhoneNumber,
                text = message,
                sender = _options.Sender,
                scheduled = "NOW",
                unicode = false
            };

            var verification = new OtpVerification
            {
                Contact = phoneNumber,
                Code = code,
                ExpirationDate = DateTime.UtcNow.AddMinutes(10),
                IsVerified = false,
                ContactType = ContactType.Phone
            };

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber) ?? throw new ArgumentException($"User with phone number {phoneNumber} does not exist.");
            var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_url, content);
            var result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                await _repository.AddAsync(verification);
                await _appLogger.LogAsync(
                     action: "Send Phone verification",
                     relatedEntityId: verification.Id.ToString(),
                     userId: user.Id,
                     userName: $"{user.FirstName} {user.LastName}",
                     details: $"Sent OTP verification code to number: ${phoneNumber}"
                );

                await _repository.SaveChangesAsync();
                return result;
            }
            throw new ArgumentException(result);
        }

        private static string CalculateMD5Hash(string input)
        {
            using var md5 = MD5.Create();
            var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sb = new StringBuilder();
            foreach (var t in bytes)
                sb.Append(t.ToString("x2"));
            return sb.ToString();
        }

        private static string GenerateOtpCode()
        {
            var random = new Random();
            return random.Next(1000, 10000).ToString();
        }

        public async Task<string> VerifyCodeAsync(string phoneNumber, string code)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber)
                ?? throw new ArgumentException($"User with phone number {phoneNumber} does not exist.");

            var verification = await _repository.GetLatestUnverifiedCodeAsync(phoneNumber, code) ?? throw new ArgumentException($"OTP code is already used or expired");
            if (verification.ExpirationDate < DateTime.UtcNow)
                throw new ArgumentException($"OTP code is expired");

            verification.IsVerified = true;

            // Optional: mark user's phone as confirmed
            if (!user.PhoneNumberConfirmed)
            {
                user.PhoneNumberConfirmed = true;
                await _userManager.UpdateAsync(user);
            }

            _repository.Update(verification);
            await _repository.SaveChangesAsync();

            await _appLogger.LogAsync(
                action: "Verify Phone",
                relatedEntityId: verification.Id.ToString(),
                userId: user.Id,
                userName: $"{user.FirstName} {user.LastName}",
                details: $"Verified OTP code for number: {phoneNumber}"
            );

            return "Verification successfull";
        }

        public async Task<string> SendVerificationCodeAsync(string phoneNumber, string code)
        {
            HttpClient _httpClient = new();
            string _url = _options.PostUrl;
            var message = $"Bu, hesabın təsdiqlənməsi üçün birdəfəlik OTP kodunuzdur: {code}";
            var formattedPhoneNumber = phoneNumber.Replace("+", "").Replace(" ", "").Replace("-", "");


            string passwordMd5 = CalculateMD5Hash(_options.Password);
            string keyRaw = passwordMd5 + _options.Username + message + formattedPhoneNumber + _options.Sender;
            string key = CalculateMD5Hash(keyRaw);

            var body = new
            {
                login = _options.Username,
                key,
                msisdn = formattedPhoneNumber,
                text = message,
                sender = _options.Sender,
                scheduled = "NOW",
                unicode = false
            };

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber) ?? throw new ArgumentException($"User with phone number {phoneNumber} does not exist.");
            var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_url, content);
            var result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return result;
            }
            throw new ArgumentException(result);
        }
    }
}
