using Dapper;
using CRM.ApiHub.Domain.Entities;
using CRM.ApiHub.Domain.Repositories;

namespace CRM.ApiHub.Infrastructure.Persistence;

public class FormRepository : IFormRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public FormRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<FormTemplate>> GetTemplatesByCampaignStageAsync(long idCmpg, long idStage)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT * FROM sales_service.sales_form_template 
            WHERE id_cmpg = @IdCmpg AND id_stage = @IdStage AND is_active = true 
            ORDER BY form_order;";
            
        return await connection.QueryAsync<FormTemplate>(sql, new { IdCmpg = idCmpg, IdStage = idStage });
    }

    public async Task<IEnumerable<FormField>> GetFieldsByTemplateAsync(long idForm)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT * FROM sales_service.sales_form_field 
            WHERE id_form = @IdForm AND is_active = true 
            ORDER BY order_index;";
            
        return await connection.QueryAsync<FormField>(sql, new { IdForm = idForm });
    }

    public async Task SaveOrderDataAsync(long idOrder, long idForm, IEnumerable<OrderData> fields)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO sales_service.sales_order_data 
            (id_order, id_fld, value_text, value_json, field_status, version, source_form_id, register) 
            VALUES 
            (@IdOrder, @IdFld, @ValueText, @ValueJson::jsonb, @FieldStatus, 1, @SourceFormId, NOW());";

        foreach (var field in fields)
        {
            field.IdOrder = idOrder;
            field.SourceFormId = idForm;
            if (string.IsNullOrEmpty(field.FieldStatus)) field.FieldStatus = "PENDING"; 
        }
        
        await connection.ExecuteAsync(sql, fields);
    }
}