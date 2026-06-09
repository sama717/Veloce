using Microsoft.EntityFrameworkCore;
using Veloce.Repository;
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

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddAutoMapper(cfg =>
            {
                cfg.LicenseKey = licenseKey;
            },
        typeof(Program).Assembly);
        builder.Services.AddDbContext<VeloceDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });
        
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<ICarRepository, CarRepository>();
        builder.Services.AddScoped<IBookingRepository, BookingRepository>();
        builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
        builder.Services.AddScoped<IDealershipRepository, DealershipRepository>();
        builder.Services.AddScoped<IAssetOwnershipRepository, AssetOwnershipRepository>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}