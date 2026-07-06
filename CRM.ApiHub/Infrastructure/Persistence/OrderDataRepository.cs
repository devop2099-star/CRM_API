using Dapper;
using CRM.ApiHub.Domain.Entities;
using CRM.ApiHub.Domain.Repositories;

namespace CRM.ApiHub.Infrastructure.Persistence;

public class OrderDataRepository : IOrderDataRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public OrderDataRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<OrderData>> GetByOrderAsync(long idOrder)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "SELECT * FROM sales_service.sales_order_data WHERE id_order = @IdOrder;";
        return await connection.QueryAsync<OrderData>(sql, new { IdOrder = idOrder });
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

    public async Task UpdateFieldStatusAsync(long idData, string status, long validatedBy)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE sales_service.sales_order_data 
            SET field_status = @Status, 
                validated_by = @ValidatedBy, 
                validated_at = NOW() 
            WHERE id_orddata = @IdData;";
            
        await connection.ExecuteAsync(sql, new { Status = status, ValidatedBy = validatedBy, IdData = idData });
    }
}