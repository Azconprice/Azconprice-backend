using Application.Models.DTOs;
using Application.Models.DTOs.Company;
using Application.Models.DTOs.User;
using Application.Models.DTOs.Worker;
using Application.Repositories;
using Application.Services;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        RoleManager<IdentityRole> roleManager,
        IJWTService jwtService,
        IMailService mailService,
        IValidator<RegisterWorkerRequest> registerWorkerValidator,
        IValidator<RegisterUserRequest> registerUserValidator,
        IValidator<RegisterCompanyRequest> registerCompanyValidator,
        IWorkerProfileRepository workerProfileRepository,
        ICompanyProfileRepository companyProfileRepository,
        IBucketService bucketService,
        IWorkerService workerService
    ) : ControllerBase
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly SignInManager<User> _signInManager = signInManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly IJWTService _jwtService = jwtService;
        private readonly IMailService _mailService = mailService;
        private readonly IValidator<RegisterWorkerRequest> _registerWorkerValidator = registerWorkerValidator;
        private readonly IValidator<RegisterUserRequest> _registerUserValidator = registerUserValidator;
        private readonly IValidator<RegisterCompanyRequest> _registerCompanyValidator = registerCompanyValidator;
        private readonly IWorkerProfileRepository _workerProfileRepository = workerProfileRepository;
        private readonly ICompanyProfileRepository _companyProfileRepository = companyProfileRepository;
        private readonly IBucketService _bucketService = bucketService;
        private readonly IWorkerService _workerService = workerService;

        private async Task<AuthTokenDTO> GenerateToken(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = await _userManager.GetClaimsAsync(user);

            var accessToken = _jwtService.GenerateSecurityToken(user.Id, user.Email, roles, claims);

            var refreshToken = Guid.NewGuid().ToString("N").ToLower();

            user.RefreshToken = refreshToken;
            await _userManager.UpdateAsync(user);

            return new AuthTokenDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };
        }

        [HttpPost("register/user")]
        public async Task<ActionResult> RegisterUser([FromForm] RegisterUserRequest request)
        {
            var validationResult = await _registerUserValidator.ValidateAsync(request);

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

                // 🔍 Wrap Cloudflare upload in try-catch
                try
                {
                    profilePictureUrl = await _bucketService.UploadAsync(request.ProfilePicture);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Upload failed: {ex.Message}");
                    return StatusCode(500, "Image upload failed.");
                }
            }

            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser is not null)
                return Conflict("User already exists");

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.Email,
                Email = request.Email,
                RefreshToken = Guid.NewGuid().ToString("N").ToLower(),
                ProfilePicture = profilePictureUrl,
                PhoneNumber = request.PhoneNumber
            };
            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var url = Url.Action(nameof(ConfirmEmail), "Auth", new { email = user.Email, token = confirmToken }, Request.Scheme);
            if (url is not null)
                _mailService.SendConfirmationMessage(user.Email, url);

            await _userManager.AddToRoleAsync(user, "User");

            return Ok();
        }

        [HttpPost("register/worker")]
        [RequestSizeLimit(50_000_000)] // 50 MB
        public async Task<IActionResult> RegisterWorker([FromForm] RegisterWorkerRequest request)
        {
            try
            {
                var validationResult = await _registerWorkerValidator.ValidateAsync(request);

                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    return BadRequest(new { Errors = errors });
                }

                // Specialization validation
                if (request.Specizalizations != null && request.Specizalizations.Any())
                {
                    if (!await _workerService.AreSpecializationsValid(request.Specizalizations))
                        return BadRequest(new { Error = "One or more specialization IDs are invalid." });
                }

                string? profilePictureUrl = null;
                if (request.ProfilePicture != null)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(request.ProfilePicture.FileName);

                    // 🔍 Wrap Cloudflare upload in try-catch
                    try
                    {
                        profilePictureUrl = await _bucketService.UploadAsync(request.ProfilePicture);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ Upload failed: {ex.Message}");
                        return StatusCode(500, "Image upload failed.");
                    }
                }

                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser is not null)
                    return Conflict("User already exists");

                var user = new User
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    UserName = request.Email,
                    Email = request.Email,
                    RefreshToken = Guid.NewGuid().ToString("N").ToLower(),
                    ProfilePicture = profilePictureUrl,
                    PhoneNumber = request.PhoneNumber
                };

                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                    return BadRequest(result.Errors);

                var workerProfile = new WorkerProfile
                {
                    UserId = user.Id,
                    HaveTaxId = request.HaveTaxId,
                    TaxId = request.TaxId,
                    Address = request.Address,
                    Experience = request.Experience,
                    Price = request.Price,
                };

                await _workerProfileRepository.AddAsync(workerProfile);
                await _workerProfileRepository.SaveChangesAsync();

                var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var url = Url.Action(nameof(ConfirmEmail), "Auth", new { email = user.Email, token = confirmToken }, Request.Scheme);
                if (url is not null)
                    _mailService.SendConfirmationMessage(user.Email, url);

                await _userManager.AddToRoleAsync(user, "Worker");

                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine($"🔥 CRASH: {e}");
                return StatusCode(500, "Unhandled server error.");
            }
        }

        [HttpPost("register/company")]
        public async Task<IActionResult> RegisterCompany([FromForm] RegisterCompanyRequest request)
        {
            var validationResult = await _registerCompanyValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new { Errors = errors });
            }

            string? logo = null;
            if (request.Logo != null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(request.Logo.FileName);

                // 🔍 Wrap Cloudflare upload in try-catch
                try
                {
                    logo = await _bucketService.UploadAsync(request.Logo);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Upload failed: {ex.Message}");
                    return StatusCode(500, "Image upload failed.");
                }
            }

            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser is not null)
                return Conflict("User already exists");

            var user = new User
            {
                FirstName = request.CompanyName,
                LastName = request.CompanyName,
                UserName = request.Email,
                Email = request.Email,
                RefreshToken = Guid.NewGuid().ToString("N").ToLower(),
                PhoneNumber = request.PhoneNumber
            };
            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            var companyProfile = new CompanyProfile
            {
                UserId = user.Id,
                TaxId = request.TaxId,
                Address = request.Address,
                CompanyLogo = logo,
                IsConfirmed= false
            };
            await _companyProfileRepository.AddAsync(companyProfile);
            await _companyProfileRepository.SaveChangesAsync();

            var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var url = Url.Action(nameof(ConfirmEmail), "Auth", new { email = user.Email, token = confirmToken }, Request.Scheme);
            if (url is not null)
                _mailService.SendConfirmationMessage(user.Email, url);

            await _userManager.AddToRoleAsync(user, "Company");

            return Ok();
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthTokenDTO>> Login(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
            {
                return BadRequest();
            }
            if (await _userManager.IsEmailConfirmedAsync(user))
            {
                var canSignIn = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

                if (!canSignIn.Succeeded)
                    return BadRequest();

                return await GenerateToken(user);
            }
            return Unauthorized();
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthTokenDTO>> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(e => e.RefreshToken == request.RefreshToken);

            if (user is null)
                return Unauthorized();

            return await GenerateToken(user);
        }

        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is not null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
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