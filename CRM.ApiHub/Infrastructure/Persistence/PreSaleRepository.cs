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
            INSERT INTO lead_service.lead_pre_sale (
                id_cmpg, phone, operator, first_name, last_name, address, province, 
                coverage_status, id_status, owner_user_id, current_user_id, notes, register
            )
            VALUES (
                @IdCmpg, @Phone, @Operator, @FirstName, @LastName, @Address, @Province, 
                @CoverageStatus, @IdStatus, @OwnerUserId, @CurrentUserId, @Notes, @Register
            )
            RETURNING id_presale;";

        return await connection.ExecuteScalarAsync<int>(sql, preSale);
    }

    public async Task<bool> AddCallLogAsync(int idPresale, string callLog, long userId = 1)
    {
        using var connection = _connectionFactory.CreateConnection();

        // 1. Calculate the next call_number
        const string getNextCallNumSql = "SELECT COALESCE(MAX(call_number), 0) + 1 FROM lead_service.lead_call_log WHERE id_presale = @PreSaleId;";
        var nextCallNum = (short)await connection.ExecuteScalarAsync<int>(getNextCallNumSql, new { PreSaleId = idPresale });

        // 2. Insert call log
        const string sql = @"
            INSERT INTO lead_service.lead_call_log 
            (id_presale, call_number, id_user, call_type, data_obtained, notes, register)
            VALUES 
            (@PreSaleId, @CallNumber, @UserId, @CallType, @DataObtained::jsonb, @Notes, @Register);";

        var rowsAffected = await connection.ExecuteAsync(sql, new 
        { 
            PreSaleId = idPresale, 
            CallNumber = nextCallNum,
            UserId = userId,
            CallType = "OUTBOUND",
            DataObtained = "{}",
            Notes = callLog,
            Register = DateTime.UtcNow
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