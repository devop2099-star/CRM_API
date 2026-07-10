using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CRM.ApiHub.Domain.Entities;
using CRM.ApiHub.Domain.Repositories;
using Dapper;

namespace CRM.ApiHub.Infrastructure.Persistence;

public class FormSchemaRepository : IFormSchemaRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public FormSchemaRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<SalesFormTemplate?> GetTemplateAsync(long idCmpg, long idStatus, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT * FROM sales_service.sales_form_template 
            WHERE id_cmpg = @IdCmpg AND id_status = @IdStatus AND is_active = true
            LIMIT 1;";
            
        return await connection.QueryFirstOrDefaultAsync<SalesFormTemplate>(
            new CommandDefinition(sql, new { IdCmpg = idCmpg, IdStatus = idStatus }, cancellationToken: ct)
        );
    }

    public async Task<IEnumerable<SalesFormField>> GetFieldsByTemplateIdAsync(long idForm, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT * FROM sales_service.sales_form_field 
            WHERE id_form = @IdForm AND is_active = true
            ORDER BY order_index ASC;";
            
        return await connection.QueryAsync<SalesFormField>(
            new CommandDefinition(sql, new { IdForm = idForm }, cancellationToken: ct)
        );
    }
}
