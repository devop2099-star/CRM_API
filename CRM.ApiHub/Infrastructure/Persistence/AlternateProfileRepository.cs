using Dapper;
using System.Threading.Tasks; // Necesario para Task
using CRM.ApiHub.Domain.Entities; 
using CRM.ApiHub.Domain.Repositories; 

namespace CRM.ApiHub.Infrastructure.Persistence
{
    public class AlternateProfileRepository : IAlternateProfileRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        // Constructor inyectando la factoría correcta
        public AlternateProfileRepository(IDbConnectionFactory dbConnectionFactory)        
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<long> CreateAsync(AlternateProfile entity)
        {
            var sql = @"INSERT INTO sales_service.order_alternate_profile 
                        (id_order, alternate_type, alternate_data, original_data, reason, created_by, is_active, created_at) 
                        VALUES (@IdOrder, @AlternateType, @AlternateData::jsonb, @OriginalData::jsonb, @Reason, @CreatedBy, true, NOW()) 
                        RETURNING id_alternate;";
            
            using var connection = await _dbConnectionFactory.CreateConnectionAsync();
            return await connection.ExecuteScalarAsync<long>(sql, entity);
        }
    }
}