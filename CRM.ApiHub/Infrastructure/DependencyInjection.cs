using System.Text;
using CRM.ApiHub.Application.Interfaces;
using CRM.ApiHub.Application.UseCases.Auth;
using CRM.ApiHub.Domain.Repositories;
using CRM.ApiHub.Infrastructure.Authentication;
using CRM.ApiHub.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace CRM.ApiHub.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        // DB
        services.AddSingleton<IDbConnectionFactory, NpgsqlConnectionFactory>();

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICampaignRepository, CampaignRepository>();
        services.AddScoped<ICatalogRepository, CatalogRepository>();

        // Services
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        
        // Use Cases
        services.AddScoped<LoginUseCase>();

        // JWT
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters 
                {
                    ValidateIssuer           = true,
                    ValidateAudience         = true,
                    ValidateLifetime         = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer   = config["JwtSettings:Issuer"],
                    ValidAudience = config["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(config["JwtSettings:SecretKey"] ?? "a-default-secret-key-that-is-long-enough-for-validation-32-chars-long"))
                };
            });

        return services;
    }
}