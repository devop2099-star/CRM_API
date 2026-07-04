using Dapper;
using System.Data;
using CRM.ApiHub.Domain.Entities;
using CRM.ApiHub.Domain.Repositories;

namespace CRM.ApiHub.Infrastructure.Persistence;

public class CampaignRepository : ICampaignRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public CampaignRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Campaign>> GetAllActiveAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "SELECT * FROM campaign WHERE is_active = true;";
        
        return await connection.QueryAsync<Campaign>(sql);
    }

    public async Task<Campaign?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "SELECT * FROM campaign WHERE id = @Id;";
        
        // Usamos un query parametrizado pasándole un objeto anónimo
        return await connection.QueryFirstOrDefaultAsync<Campaign>(sql, new { Id = id });
    }
}