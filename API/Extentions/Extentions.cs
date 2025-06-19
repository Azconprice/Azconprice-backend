using Amazon.Runtime;
using Amazon.S3;
using Application.Models;
using Application.Repositories;
using Application.Services;
using Application.Validators.Worker;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Persistence.Contexts;
using Persistence.Repositories;
using Supabase;
using System.Text;

namespace API.Extentions
{
    public static class Extentions
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(setup =>
            {
                setup.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "My Api - V1",
                        Version = "v1",
                    }
                );

                setup.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Jwt Authorization header using the Bearer scheme/ \r\r\r\n Enter 'Bearer' [space] and then token in the text input below. \r\n\r\n Example : \"Bearer f3c04qc08mh3n878\""
                });

                setup.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id ="Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
            return services;
        }

        public static async void SeedRolesAsync(this WebApplication app)
        {
            var container = app.Services.CreateScope();
            var userManager = container.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = container.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            if (!await roleManager.RoleExistsAsync("Worker"))
                await roleManager.CreateAsync(new IdentityRole("Worker"));
            if (!await roleManager.RoleExistsAsync("User"))
                await roleManager.CreateAsync(new IdentityRole("User"));
            if (!await roleManager.RoleExistsAsync("Company"))
                await roleManager.CreateAsync(new IdentityRole("Company"));

            var user = await userManager.FindByEmailAsync("admin@admin.com");
            if (user is null)
            {
                user = new User
                {
                    FirstName = "admin",
                    LastName = "admin",
                    UserName = "admin@admin.com",
                    Email = "admin@admin.com",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(user, "Admin_2924");
                result = await userManager.AddToRoleAsync(user, "Admin");
            }
        }

        public static IServiceCollection AddAuthenticationAndAuthorization(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<User, IdentityRole>(op =>
            {
                op.Password.RequiredLength = 3;
                op.Password.RequireNonAlphanumeric = false;
                op.Password.RequireUppercase = false;
                op.Password.RequireLowercase = false;
                op.Password.RequireDigit = false;
                op.Lockout.AllowedForNewUsers = true;
                op.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
                op.Lockout.MaxFailedAccessAttempts = 5;
            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

            services.Configure<DataProtectionTokenProviderOptions>(o =>
            {
                o.TokenLifespan = TimeSpan.FromHours(1);
            });

            var jwtConfig = new JWTConfig
            {
                Secret = Environment.GetEnvironmentVariable("JWT__Secret")!,
                Issuer = Environment.GetEnvironmentVariable("JWT__Issuer")!,
                Audience = Environment.GetEnvironmentVariable("JWT__Audience")!,
                ExpireMunites = int.Parse(Environment.GetEnvironmentVariable("JWT__ExpiresInMinutes") ?? "40")
            };

            services.AddSingleton(jwtConfig);
            services.AddScoped<IJWTService, JWTService>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(setup =>
            {
                setup.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidAudience = jwtConfig.Audience,
                    ValidIssuer = jwtConfig.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Secret))
                };
            });

            services.AddAuthorization();
            return services;
        }

        public static IServiceCollection AddContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connection = Environment.GetEnvironmentVariable("ConnectionStrings__Default");
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connection).UseLazyLoadingProxies());
            return services;
        }

        public static IServiceCollection AddQuickSMSService(this IServiceCollection services, IConfiguration configuration)
        {
            var quickSMSOptions = new QuickSMSOptions
            {
                Username = Environment.GetEnvironmentVariable("QuickSMS__Username")!,
                Password = Environment.GetEnvironmentVariable("QuickSMS__Password")!,
                Sender = Environment.GetEnvironmentVariable("QuickSMS__Sender")!,
                PostUrl = Environment.GetEnvironmentVariable("QuickSMS__PostUrl")!
            };
            services.AddSingleton(quickSMSOptions);
            services.AddScoped<ISMSService, QuickSMSService>();
            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IProfessionRepository, ProfessionRepository>();
            services.AddScoped<ISpecializationRepository, SpecializationRepository>();
            services.AddScoped<IWorkerProfileRepository, WorkerProfileRepository>();
            services.AddScoped<ICompanyProfileRepository, CompanyProfileRepository>();
            services.AddScoped<IAppLogRepository, AppLogRepository>();
            services.AddScoped<IRequestRepository, RequestRepository>();
            services.AddScoped<ISalesCategoryRepository, SalesCategoryRepository>();
            services.AddScoped<IExcelFileRecordRepository, ExcelFileRecordRepository>();
            services.AddScoped<IPhoneVerificationRepository,PhoneVerificationRepository>();
            return services;
        }

        public static IServiceCollection AddDomainServices(this IServiceCollection services, IConfiguration configuration)
        {
            var smtpConfig = new SMTPConfig
            {
                Host = Environment.GetEnvironmentVariable("SMTP__Host")!,
                Port = int.Parse(Environment.GetEnvironmentVariable("SMTP__Port") ?? "587"),
                Username = Environment.GetEnvironmentVariable("SMTP__Username")!,
                Password = Environment.GetEnvironmentVariable("SMTP__Password")!,
                EnableSsl = bool.Parse(Environment.GetEnvironmentVariable("SMTP__EnableSsl") ?? "true")
            };
            services.AddSingleton(smtpConfig);

            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IWorkerService, WorkerService>();
            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<IProfessionService, ProfessionService>();
            services.AddScoped<ISpecializationService, SpecializationService>();
            services.AddScoped<IRequestService, RequestService>();
            services.AddScoped<IAppLogger, AppLogger>();
            services.AddScoped<ISalesCategoryService, SalesCategoryService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IMailService, MailService>();
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddScoped<IExcelFileService, ExcelFileService>();
            return services;
        }

        public static IServiceCollection AddSupabaseStorage(this IServiceCollection services, IConfiguration config)
        {
            var supabaseSettings = new SupabaseSettings
            {
                Url = Environment.GetEnvironmentVariable("Supabase__Url")!,
                ApiKey = Environment.GetEnvironmentVariable("Supabase__ApiKey")!,
                BucketName = Environment.GetEnvironmentVariable("Supabase__BucketName")!
            };

            services.AddSingleton(supabaseSettings);

            services.AddSingleton<Client>(sp =>
            {
                var options = new Supabase.SupabaseOptions { AutoConnectRealtime = false };
                var client = new Supabase.Client(supabaseSettings.Url, supabaseSettings.ApiKey, options);
                client.InitializeAsync().Wait();
                return client;
            });

            services.AddScoped<IBucketService, SupabaseStorageService>();
            return services;
        }

        public static IServiceCollection AddValidators(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<RegisterWorkerRequestValidator>();
            return services;
        }
    }
}
