using System.Text;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Veloce.Mapping;
using Veloce.Middleware;
using Veloce.Repository;
using Veloce.Services;
using Veloco.Data;
using Veloco.Interfaces;

namespace Veloce;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var licenseKey = builder.Configuration["Automapper:LicenseKey"];
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        var jwtKey =  builder.Configuration["Jwt:Key"];
        var jwtIssuer = builder.Configuration["Jwt:Issuer"];
        var jwtAudience = builder.Configuration["Jwt:Audience"];
        var cloudinarySettings = builder.Configuration.GetSection("Cloudinary");
        var cloudinary = new Cloudinary(new Account(
            cloudinarySettings["CloudName"],
            cloudinarySettings["ApiKey"],
            cloudinarySettings["ApiSecret"]
        ));

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddAutoMapper(cfg =>
            {
                cfg.LicenseKey = licenseKey;
            },
        typeof(MappingProfile).Assembly);
        builder.Services.AddDbContext<VeloceDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });
        
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<ICarRepository, CarRepository>();
        builder.Services.AddScoped<IBookingRepository, BookingRepository>();
        builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
        builder.Services.AddScoped<IDealershipRepository, DealershipRepository>();
        builder.Services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
        builder.Services.AddScoped<IEmailChangeTokenRepository, EmailChangeTokenRepository>();
        builder.Services.AddScoped<IPhoneChangeTokenRepository, PhoneChangeTokenRepository>();
        builder.Services.AddScoped<IAssetOwnershipRepository, AssetOwnershipRepository>();
        builder.Services.AddScoped<IEmailVerificationTokenRepository, EmailVerificationTokenRepository>();
        builder.Services.AddScoped<IEmployeeProfileRepository, EmployeeProfileRepository>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<ITokenGenerator, TokenGenerator>();
        builder.Services.AddSingleton(cloudinary);
        builder.Services.AddScoped<IImageService, ImageService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<ICarService, CarService>();
        builder.Services.AddScoped<IDealershipService, DealershipService>();
        builder.Services.AddScoped<IBookingService, BookingService>();
        builder.Services.AddScoped<IPaymentService, PaymentService>();
        builder.Services.AddScoped<IRentalContractService, RentalContractService>();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtKey))
            };
        });
        
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        
        app.UseAuthentication();

        app.UseAuthorization();
        
        app.MapControllers();

        app.Run();
    }
}