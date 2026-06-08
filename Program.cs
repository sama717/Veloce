using Microsoft.EntityFrameworkCore;
using Veloco.Data;
namespace Veloco;

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