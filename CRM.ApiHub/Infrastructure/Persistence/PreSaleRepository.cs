using Dapper;
using System.Data;
using CRM.ApiHub.Domain.Entities;
using CRM.ApiHub.Domain.Repositories;

namespace CRM.ApiHub.Infrastructure.Persistence;

public class PreSaleRepository : IPreSaleRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public PreSaleRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<LeadPreSale>> GetByUserAsync(int userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "SELECT * FROM lead_service.lead_pre_sale WHERE user_id = @UserId;";
        
        return await connection.QueryAsync<LeadPreSale>(sql, new { UserId = userId });
    }

    public async Task<int> CreateAsync(LeadPreSale preSale)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO lead_service.lead_pre_sale (lead_id, user_id, campaign_id, status, created_at)
            VALUES (@LeadId, @UserId, @CampaignId, @Status, @CreatedAt)
            RETURNING id;";

        return await connection.ExecuteScalarAsync<int>(sql, preSale);
    }

    public async Task<bool> AddCallLogAsync(int idPresale, string callLog)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO lead_service.lead_call_log (lead_pre_sale_id, log_details, created_at)
            VALUES (@PreSaleId, @LogDetails, @CreatedAt);";

        var rowsAffected = await connection.ExecuteAsync(sql, new 
        { 
            PreSaleId = idPresale, 
            LogDetails = callLog,
            CreatedAt = DateTime.UtcNow
        });

        return rowsAffected > 0;
    }

    public async Task<bool> AssignAsync(int idPresale, int toUserId, string context)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE lead_service.lead_pre_sale 
            SET user_id = @ToUserId, assignment_context = @Context, updated_at = @UpdatedAt
            WHERE id = @IdPresale;";

        var rowsAffected = await connection.ExecuteAsync(sql, new 
        { 
            ToUserId = toUserId, 
            Context = context, 
            IdPresale = idPresale,
            UpdatedAt = DateTime.UtcNow
        });

        return rowsAffected > 0;
    }

    public async Task<bool> ConvertAsync(int idPresale, dynamic paramsData)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE lead_service.lead_pre_sale 
            SET status = 'Converted', updated_at = @UpdatedAt
            WHERE id = @IdPresale;";

        var rowsAffected = await connection.ExecuteAsync(sql, new 
        { 
            IdPresale = idPresale,
            UpdatedAt = DateTime.UtcNow
        });

        return rowsAffected > 0;
    }
}