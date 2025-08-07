using Application.Models.DTOs;
using Application.Models.DTOs.Company;
using Application.Models.DTOs.User;
using Application.Models.DTOs.Worker;
using Application.Repositories;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IJWTService jwtService,
        IMailService mailService,
        IValidator<RegisterWorkerRequest> registerWorkerValidator,
        IValidator<RegisterUserRequest> registerUserValidator,
        IValidator<RegisterCompanyRequest> registerCompanyValidator,
        IWorkerProfileRepository workerProfileRepository,
        ICompanyProfileRepository companyProfileRepository,
        IBucketService bucketService,
        IWorkerService workerService,
        IAppLogger appLogger,
        ICompanyService companyService,
        ISMSService smsService,
        IValidator<SendPhoneVerificationRequest> sendPhoneVerificationRequestValidator,
        IOtpVerificationRepository otpVerificationRepository,
        IWorkerFunctionRepository workerFunctionRepository,
        IWorkerFunctionSpecializationService workerFunctionSpecializationService) : ControllerBase
    {

        private async Task<AuthTokenDTO> GenerateToken(User user)
        {
            var roles = await userManager.GetRolesAsync(user);
            var claims = await userManager.GetClaimsAsync(user);

            var accessToken = jwtService.GenerateSecurityToken(user.Id, user.Email, user.FirstName, user.LastName, roles, claims);

            var refreshToken = Guid.NewGuid().ToString("N").ToLower();

            user.RefreshToken = refreshToken;
            await userManager.UpdateAsync(user);

            return new AuthTokenDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };
        }

        [HttpPost("otp-create")]
        public async Task<IActionResult> SendPhoneVerification([FromBody] SendPhoneVerificationRequest request)
        {
            var validationResult = await sendPhoneVerificationRequestValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new { Errors = errors });
            }
            try
            {
                var status = await smsService.SendVerificationCodeAsync(request.PhoneNumber);
                return Ok(status);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("otp-verify")]
        public async Task<IActionResult> VerifyPhone([FromBody] VerifyPhoneRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.PhoneNumber) || string.IsNullOrWhiteSpace(request.Code))
                return BadRequest("Phone number and code are required.");

            try
            {
                var status = await smsService.VerifyCodeAsync(request.PhoneNumber, request.Code);
                return Ok(status);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }


        }

        [HttpPost("register/user")]
        public async Task<ActionResult> RegisterUser([FromForm] RegisterUserRequest request)
        {
            var validationResult = await registerUserValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new { Errors = errors });
            }

            string? profilePictureUrl = null;
            if (request.ProfilePicture != null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(request.ProfilePicture.FileName);

                try
                {
                    profilePictureUrl = await bucketService.UploadAsync(request.ProfilePicture);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Upload failed: {ex.Message}");
                    return StatusCode(500, "Image upload failed.");
                }
            }

            var existingUser = await userManager.FindByEmailAsync(request.Email);
            if (existingUser is not null)
                return Conflict("User with this email already exists");

            existingUser = await userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber.Replace(" ", "").Replace("-", ""));
            if (existingUser is not null)
                return Conflict("User with this phone number already exists");

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.Email,
                Email = request.Email,
                RefreshToken = Guid.NewGuid().ToString("N").ToLower(),
                ProfilePicture = profilePictureUrl,
                PhoneNumber = request.PhoneNumber.Replace(" ", "").Replace("-", "")
            };
            var result = await userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await userManager.AddToRoleAsync(user, "User");

            await appLogger.LogAsync(
            action: "User Registered",
            relatedEntityId: user.Id,
            userId: user.Id,
            userName: $"{user.FirstName} {user.LastName}",
            details: $"User registered with email {user.Email}"
            );

            return Ok();
        }



        [HttpPost("resend-confirmation")]
        public async Task<IActionResult> ResendConfirmation([FromBody] SendEmailConfrimation emailConfrimation)
        {
            if (string.IsNullOrWhiteSpace(emailConfrimation.Email))
                return BadRequest("Email is required.");

            var user = await userManager.FindByEmailAsync(emailConfrimation.Email);
            if (user is null)
                return BadRequest("User with this email does not exist.");

            if (await userManager.IsEmailConfirmedAsync(user))
                return BadRequest("Email is already confirmed.");

            var confirmToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var url = Url.Action(nameof(ConfirmEmail), "Auth", new { email = user.Email, token = confirmToken }, Request.Scheme);

            if (url is not null)
            {
                mailService.SendConfirmationMessage(user.Email, url);

                await appLogger.LogAsync(
                    action: "Resent Email Confirmation",
                    relatedEntityId: user.Id,
                    userId: user.Id,
                    userName: $"{user.FirstName} {user.LastName}",
                    details: $"Resent confirmation email to {user.Email}"
                );
            }

            return Ok("Confirmation email sent.");
        }

        [HttpPost("register/worker")]
        public async Task<IActionResult> RegisterWorker([FromForm] RegisterWorkerRequest request)
        {
            try
            {
                var validationResult = await registerWorkerValidator.ValidateAsync(request);

                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    return BadRequest(new { Errors = errors });
                }

                // Specialization validation
                if (request.Specizalizations != null && request.Specizalizations.Any())
                {
                    if (!await workerService.AreSpecializationsValid(request.Specizalizations))
                        return BadRequest(new { Error = "One or more specialization IDs are invalid." });
                }

                var existingUser = await userManager.FindByEmailAsync(request.Email);
                if (existingUser is not null)
                    return Conflict("User with this email already exists");

                existingUser = await userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber.Replace(" ", "").Replace("-", ""));
                if (existingUser is not null)
                    return Conflict("User with this phone number already exists");

                string? profilePictureUrl = null;
                if (request.ProfilePicture != null)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(request.ProfilePicture.FileName);

                    try
                    {
                        profilePictureUrl = await bucketService.UploadAsync(request.ProfilePicture);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ Upload failed: {ex.Message}");
                        return StatusCode(500, "Image upload failed.");
                    }
                }

                var user = new User
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    UserName = request.Email,
                    Address = request.Address,
                    Email = request.Email,
                    RefreshToken = Guid.NewGuid().ToString("N").ToLower(),
                    ProfilePicture = profilePictureUrl,
                    PhoneNumber = request.PhoneNumber.Replace(" ", "").Replace("-", "")
                };

                var result = await userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                    return BadRequest(result.Errors);

                var workerProfile = new WorkerProfile
                {
                    UserId = user.Id,
                    HaveTaxId = request.HaveTaxId,
                    TaxId = request.TaxId,
                    Experience = request.Experience,
                    Price = request.Price,
                };


                var addedProfile = await workerProfileRepository.AddAsync(workerProfile);
                await workerFunctionRepository.SaveChangesAsync();

                var workerFunction = new WorkerFunction
                {
                    ProfessionId = Guid.Parse(request.ProfessionId),
                    MeasurementUnitId = Guid.Parse(request.MeasurementUnitId),
                    WorkerProfileId = addedProfile.Id,
                    Price = request.Price,
                    CreatedTime = DateTime.UtcNow,
                };

                var addedWorkerFunction = await workerFunctionRepository.AddAsync(workerFunction) ?? throw new Exception("Internal server error");
                await workerFunctionRepository.SaveChangesAsync();



                if (request.Specizalizations is not null && request.Specizalizations.Any())
                {
                    await workerFunctionSpecializationService.AddRangeOfSpecializationsToWorkerFunctionAsync(addedWorkerFunction.Id, request.Specizalizations);
                }

                await userManager.AddToRoleAsync(user, "Worker");

                await appLogger.LogAsync(
                action: "Worker Registered",
                relatedEntityId: user.Id,
                userId: user.Id,
                userName: $"{user.UserName}",
                details: $"Worker registered with email {user.Email}"
                );

                await workerProfileRepository.SaveChangesAsync();


                return Ok();
            }
            catch (Exception e)
            {
                var user = await userManager.FindByEmailAsync(request.Email);
                if (user is not null)
                    await userManager.DeleteAsync(user);
                Console.WriteLine($"🔥 CRASH: {e}");
                return StatusCode(500, "Unhandled server error.");
            }
        }

        [HttpPost("register/company")]
        public async Task<IActionResult> RegisterCompany([FromForm] RegisterCompanyRequest request)
        {
            var validationResult = await registerCompanyValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new { Errors = errors });
            }

            if (await companyService.IsSalesCategoryValid(request.SalesCategoryId) is false)
            {
                return BadRequest(new { Error = "Invalid sales category ID." });
            }

            var existingUser = await userManager.FindByEmailAsync(request.Email);
            if (existingUser is not null)
                return Conflict("Company with this email already exists");

            existingUser = await userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber.Replace(" ", "").Replace("-", ""));
            if (existingUser is not null)
                return Conflict("Company with this phone number already exists");

            string? logo = null;
            if (request.Logo is not null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(request.Logo.FileName);

                // 🔍 Wrap Cloudflare upload in try-catch
                try
                {
                    logo = await bucketService.UploadAsync(request.Logo);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Upload failed: {ex.Message}");
                    return StatusCode(500, "Image upload failed.");
                }
            }

            string tax;

            if (request.TaxId is not null)
            {
                var fileName = request.CompanyName + Path.GetExtension(request.TaxId.FileName);

                try
                {
                    tax = await bucketService.UploadTaxIdAsync(request.TaxId, fileName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Upload failed: {ex.Message}");
                    return StatusCode(500, "Image upload failed.");
                }
            }
            else
                return BadRequest("Tax ID file is required.");

            var user = new User
            {
                FirstName = request.CompanyName,
                LastName = request.CompanyName,
                UserName = request.CompanyName,
                Address = request.Address,
                Email = request.Email,
                RefreshToken = Guid.NewGuid().ToString("N").ToLower(),
                PhoneNumber = request.PhoneNumber.Replace(" ", "").Replace("-", "")
            };

            var result = await userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            var companyProfile = new CompanyProfile
            {
                CompanyName = request.CompanyName,
                UserId = user.Id,
                TaxId = tax,
                CompanyLogo = logo,
                IsConfirmed = false,
                SalesCategoryId = Guid.Parse(request.SalesCategoryId)
            };
            await companyProfileRepository.AddAsync(companyProfile);
            await companyProfileRepository.SaveChangesAsync();

            await userManager.AddToRoleAsync(user, "Company");

            await appLogger.LogAsync(
                action: "Company Registered",
                relatedEntityId: user.Id,
                userId: user.Id,
                userName: $"{user.UserName}",
                details: $"User registered with email {user.Email}"
                );

            return Ok();
        }

        [HttpPost("reset-password/request-otp")]
        public async Task<IActionResult> RequestPasswordResetOtp([FromBody] PasswordResetRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Contact) || string.IsNullOrWhiteSpace(dto.ContactType))
                return BadRequest("Invalid request.");

            if (!Enum.TryParse<ContactType>(dto.ContactType, true, out var contactType) ||
                (contactType != ContactType.Email && contactType != ContactType.Phone))
                return BadRequest("Invalid contact type.");

            User? user = contactType == ContactType.Email
                ? await userManager.FindByEmailAsync(dto.Contact)
                : await userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == dto.Contact);

            if (user is null)
                return BadRequest("User not found.");

            var otp = new Random().Next(100000, 999999).ToString();
            var otpVerification = new OtpVerification
            {
                Contact = dto.Contact,
                ContactType = contactType,
                Code = otp,
                ExpirationDate = DateTime.UtcNow.AddMinutes(10),
                IsVerified = false
            };
            await otpVerificationRepository.AddAsync(otpVerification);
            await otpVerificationRepository.SaveChangesAsync();

            if (contactType == ContactType.Email)
                mailService.SendPasswordResetMessage(dto.Contact, $"Your password reset code is: {otp}");
            else
                await smsService.SendVerificationCodeAsync(dto.Contact, otp);

            return Ok("OTP sent.");
        }

        [HttpPost("reset-password/verify-otp")]
        public async Task<IActionResult> VerifyPasswordResetOtp([FromBody] PasswordResetVerifyOtpDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Contact) || string.IsNullOrWhiteSpace(dto.ContactType) || string.IsNullOrWhiteSpace(dto.Otp))
                return BadRequest("Invalid request.");

            if (!Enum.TryParse<ContactType>(dto.ContactType, true, out var contactType) ||
                (contactType != ContactType.Email && contactType != ContactType.Phone))
                return BadRequest("Invalid contact type.");

            var otpRecord = await otpVerificationRepository.GetValidOtpAsync(dto.Contact, contactType, dto.Otp);
            if (otpRecord is null)
                return BadRequest("Invalid or expired OTP.");

            otpRecord.IsVerified = true;
            otpVerificationRepository.Update(otpRecord);
            await otpVerificationRepository.SaveChangesAsync();

            return Ok("OTP verified. You may now reset your password.");
        }

        [HttpPost("reset-password/confirm")]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordResetConfirmDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Contact) || string.IsNullOrWhiteSpace(dto.ContactType) || string.IsNullOrWhiteSpace(dto.NewPassword))
                return BadRequest("Invalid request.");

            if (!Enum.TryParse<ContactType>(dto.ContactType, true, out var contactType) ||
                (contactType != ContactType.Email && contactType != ContactType.Phone))
                return BadRequest("Invalid contact type.");

            var otpRecord = await otpVerificationRepository.GetLatestVerifiedCodeAsync(dto.Contact, contactType);
            if (otpRecord is null)
                return BadRequest("OTP verification required.");

            User? user = contactType == ContactType.Email
                ? await userManager.FindByEmailAsync(dto.Contact)
                : await userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == dto.Contact);

            if (user is null)
                return BadRequest("User not found.");

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var result = await userManager.ResetPasswordAsync(user, token, dto.NewPassword);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await appLogger.LogAsync(
                action: "Password Reset",
                relatedEntityId: user.Id,
                userId: user.Id,
                userName: $"{user.FirstName} {user.LastName}",
                details: $"Password reset via {(contactType == ContactType.Email ? "email" : "phone")}"
            );

            return Ok("Password reset successful.");
        }


        [HttpPost("login")]
        public async Task<ActionResult<AuthTokenDTO>> Login(LoginRequest request)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null)
            {
                return BadRequest();
            }
            if (await userManager.IsEmailConfirmedAsync(user) || user.PhoneNumberConfirmed)
            {
                var canSignIn = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);

                if (!canSignIn.Succeeded)
                    return BadRequest();

                await appLogger.LogAsync(
                    action: "Logged In",
                    relatedEntityId: user.Id,
                    userId: user.Id,
                    userName: $"{user.UserName}",
                    details: $"Logged in with email {user.Email}"
                );

                return await GenerateToken(user);
            }
            return Unauthorized();
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthTokenDTO>> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(e => e.RefreshToken == request.RefreshToken);

            if (user is null)
                return Unauthorized();

            return await GenerateToken(user);
        }

        [HttpPost("send-email-confirmation")]
        public async Task<ActionResult> SendEmailConfirmation([FromBody] SendEmailConfrimation emailConfrimation)
        {
            if (string.IsNullOrWhiteSpace(emailConfrimation.Email))
                return BadRequest("Email is required.");

            var user = await userManager.FindByEmailAsync(emailConfrimation.Email);

            if (user is null)
                return BadRequest("User with this email does not exist.");

            var confirmToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var url = Url.Action(nameof(ConfirmEmail), "Auth", new { email = user.Email, token = confirmToken }, Request.Scheme);
            if (url is not null)
                mailService.SendConfirmationMessage(user.Email, url);

            return Ok("Confirmation email sent.");
        }

        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is not null)
            {
                var result = await userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {

                    await appLogger.LogAsync(
                    action: "Email Successfully Confirmed",
                    relatedEntityId: user.Id,
                    userId: user.Id,
                    userName: $"{user.FirstName} {user.LastName}",
                    details: $"{user.Email} email of {user.FirstName} {user.LastName} successfully confirmed"
                    );
                    return Content(
                        "<html><body style='font-family:sans-serif;text-align:center;padding-top:50px;'>" +
                        "<h2>Email confirmed successfully!</h2>" +
                        "<p>You can now close this page and return to the app.</p>" +
                        "</body></html>",
                        "text/html"
                    );
                }
                else
                {
                    await appLogger.LogAsync(
                   action: "Email Confirmation failed due to token exporation",
                   relatedEntityId: user.Id,
                   userId: user.Id,
                   userName: $"{user.FirstName} {user.LastName}",
                   details: $"{user.Email} email of {user.FirstName} {user.LastName} was not confirmed"
                   );
                    return Content(
                        "<html><body style='font-family:sans-serif;text-align:center;padding-top:50px;'>" +
                        "<h2>Invalid or expired confirmation link.</h2>" +
                        "<p>Please request a new confirmation email.</p>" +
                        "</body></html>",
                        "text/html"
                    );
                }
            }
            return Content(
                "<html><body style='font-family:sans-serif;text-align:center;padding-top:50px;'>" +
                "<h2>User not found.</h2>" +
                "</body></html>",
                "text/html"
            );
        }
    }
}