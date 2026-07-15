using System.Text;
using CRM.ApiHub.Application.Interfaces;
using CRM.ApiHub.Application.UseCases.Auth;
using CRM.ApiHub.Application.UseCases.Leads;
using CRM.ApiHub.Application.UseCases.SalesOrders;
using CRM.ApiHub.Application.UseCases.Documents;
using CRM.ApiHub.Application.UseCases.Supervisor;
using CRM.ApiHub.Application.UseCases.Backoffice;
using CRM.ApiHub.Application.UseCases.Audit;
using CRM.ApiHub.Application.UseCases.KB;
using CRM.ApiHub.Domain.Repositories;
using CRM.ApiHub.Infrastructure.Authentication;
using CRM.ApiHub.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

namespace CRM.ApiHub.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        // Configuración de Dapper para mapear snake_case (db) a PascalCase (C#)
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        // DB
        services.AddSingleton<IDbConnectionFactory, NpgsqlConnectionFactory>();

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICampaignRepository, CampaignRepository>();
        services.AddScoped<ICatalogRepository, CatalogRepository>();
        services.AddScoped<IPreSaleRepository, PreSaleRepository>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<ILeadRepository, LeadRepository>();
        services.AddScoped<ISalesOrderRepository, SalesOrderRepository>();
        services.AddScoped<IOrderDocumentRepository, OrderDocumentRepository>();
        services.AddScoped<ISupervisorRepository, SupervisorRepository>();
        services.AddScoped<IBackofficeRepository, BackofficeRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IIncidentRepository, IncidentRepository>();
        services.AddScoped<IOrderDataRepository, OrderDataRepository>();
        services.AddScoped<IFormRepository, FormRepository>();
        services.AddScoped<IApprovalRepository, ApprovalRepository>();
        services.AddScoped<IAlternateProfileRepository, AlternateProfileRepository>();
        services.AddScoped<IAuditRepository, AuditRepository>();
        services.AddScoped<IKnowledgeBaseRepository, KnowledgeBaseRepository>();

        // Services & Stores
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IConnectionMultiplexer>(sp => {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var connStr = configuration["RedisSettings:ConnectionString"];
            if (string.IsNullOrEmpty(connStr)) return null!;
            try {
                return ConnectionMultiplexer.Connect(connStr);
            } catch {
                return null!; // Fallback to InMemory
            }
        });
        services.AddSingleton<IRefreshTokenStore, RedisRefreshTokenStore>();
        
        services.AddScoped<INotificationService, Application.Services.NotificationService>();
        // Use Cases
        services.AddScoped<LoginUseCase>();
        services.AddScoped<MeUseCase>();
        services.AddScoped<RefreshTokenUseCase>();
        services.AddScoped<GetLeadsUseCase>();
        services.AddScoped<GetLeadByIdUseCase>();
        services.AddScoped<CreateLeadUseCase>();
        services.AddScoped<UpdateLeadStatusUseCase>();
        
        // Sales Orders Use Cases
        services.AddScoped<GetSalesOrdersUseCase>();
        services.AddScoped<GetSalesOrderByIdUseCase>();
        services.AddScoped<CreateSalesOrderUseCase>();
        services.AddScoped<UpdateSalesOrderStatusUseCase>();
        services.AddScoped<GetSalesOrderHistoryUseCase>();

        // Document Use Cases
        services.AddScoped<GetDocumentsByOrderUseCase>();
        services.AddScoped<UploadOrderDocumentUseCase>();
        services.AddScoped<VerifyOrderDocumentUseCase>();

        // Supervisor Use Cases
        services.AddScoped<GetTeamOrdersUseCase>();
        services.AddScoped<GetTeamStatsUseCase>();
        services.AddScoped<BulkTransferToBackofficeUseCase>();

        // Backoffice Use Cases
        services.AddScoped<GetAssignedOrdersUseCase>();
        services.AddScoped<GetPendingVerificationUseCase>();
        services.AddScoped<UpdateBackofficeOrderStatusUseCase>();
        services.AddScoped<VerifyBackofficeDocumentUseCase>();

        // Audit Use Cases
        services.AddScoped<GetChecklistUseCase>();
        services.AddScoped<CreateAuditUseCase>();
        services.AddScoped<SaveAuditItemUseCase>();
        services.AddScoped<CloseAuditUseCase>();

        // KB Use Cases
        services.AddScoped<SearchKbArticlesUseCase>();
        services.AddScoped<GetKbArticleByIdUseCase>();
        services.AddScoped<SubmitKbFeedbackUseCase>();

        // JWT Authentication
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