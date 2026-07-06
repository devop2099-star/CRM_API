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
        const string sql = "SELECT * FROM lead_service.lead_pre_sale WHERE current_user_id = @UserId;";
        
        return await connection.QueryAsync<LeadPreSale>(sql, new { UserId = userId });
    }

    public async Task<int> CreateAsync(LeadPreSale preSale)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO lead_service.lead_pre_sale (id_cmpg, owner_user_id, current_user_id, id_status, register)
            VALUES (@CampaignId, @UserId, @UserId, @Status, @CreatedAt)
            RETURNING id_presale;";

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
            SET current_user_id = @ToUserId, notes = @Context, last_activity_at = @UpdatedAt
            WHERE id_presale = @IdPresale;";

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
            SET id_status = @StatusId, converted_at = @UpdatedAt, converted_by = @ConvertedBy
            WHERE id_presale = @IdPresale;";

        var rowsAffected = await connection.ExecuteAsync(sql, new 
        { 
            IdPresale = idPresale,
            StatusId = 2, 
            UpdatedAt = DateTime.UtcNow,
            ConvertedBy = paramsData?.UserId ?? 0 
        });

        return rowsAffected > 0;
    }
}