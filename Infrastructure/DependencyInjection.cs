using Application.Common;
using Application.Interfaces;
using Application.Interfaces.Security;
using Application.Interfaces.Services;
using Application.Security.Interfaces;
using Application.Services;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.Configure<JwtSettings>(
    configuration.GetSection("JwtSettings"));
        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped<IJwtProvider, JwtProvider>();

        return services;
    }
}