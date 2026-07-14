using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using CRM.ApiHub.Domain.Entities;
using CRM.ApiHub.Domain.Repositories;
using Dapper;

namespace CRM.ApiHub.Infrastructure.Persistence;

public class AuditRepository : IAuditRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public AuditRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<AuditChecklistTemplate?> GetChecklistTemplateByCampaignAsync(long idCmpg, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT 
                id_checklist AS IdChecklist,
                id_cmpg AS IdCmpg,
                name AS Name,
                version AS Version,
                is_active AS IsActive,
                created_by AS CreatedBy,
                created_at AS CreatedAt,
                deprecated_at AS DeprecatedAt
            FROM sales_service.audit_checklist_template 
            WHERE id_cmpg = @IdCmpg AND is_active = true 
            LIMIT 1;";

        return await connection.QueryFirstOrDefaultAsync<AuditChecklistTemplate>(
            new CommandDefinition(sql, new { IdCmpg = idCmpg }, cancellationToken: ct)
        );
    }

    public async Task<IEnumerable<AuditChecklistItem>> GetChecklistItemsAsync(long idChecklist, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT 
                id_item AS IdItem,
                id_checklist AS IdChecklist,
                order_index AS OrderIndex,
                item_type AS ItemType,
                description AS Description,
                expected_text AS ExpectedText,
                crm_field_key AS CrmFieldKey,
                is_critical AS IsCritical,
                ko_incident_id AS KoIncidentId,
                is_active AS IsActive
            FROM sales_service.audit_checklist_item 
            WHERE id_checklist = @IdChecklist AND is_active = true 
            ORDER BY order_index;";

        return await connection.QueryAsync<AuditChecklistItem>(
            new CommandDefinition(sql, new { IdChecklist = idChecklist }, cancellationToken: ct)
        );
    }

    public async Task<long> CreateAuditAsync(long idOrder, long idChecklist, long auditedBy, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO sales_service.sales_order_audit 
                (id_order, id_checklist, audited_by, audit_status, started_at) 
            VALUES 
                (@IdOrder, @IdChecklist, @AuditedBy, 'IN_PROGRESS', NOW()) 
            RETURNING id_audit;";

        return await connection.ExecuteScalarAsync<long>(
            new CommandDefinition(sql, new { IdOrder = idOrder, IdChecklist = idChecklist, AuditedBy = auditedBy }, cancellationToken: ct)
        );
    }

    public async Task<bool> SaveItemAsync(long idAudit, long idItem, string result, string? obs, string? audioTimestamp, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        if (connection.State != ConnectionState.Open)
        {
            connection.Open();
        }

        using var transaction = connection.BeginTransaction();
        try
        {
            // Check if it already exists
            const string selectSql = @"
                SELECT id_audit_item 
                FROM sales_service.sales_order_audit_item 
                WHERE id_audit = @IdAudit AND id_item = @IdItem;";

            var existingId = await connection.ExecuteScalarAsync<long?>(
                new CommandDefinition(selectSql, new { IdAudit = idAudit, IdItem = idItem }, transaction: transaction, cancellationToken: ct)
            );

            if (existingId.HasValue)
            {
                const string updateSql = @"
                    UPDATE sales_service.sales_order_audit_item
                    SET result = @Result, 
                        observation = @Observation, 
                        audio_timestamp = @AudioTimestamp, 
                        register = NOW()
                    WHERE id_audit_item = @IdAuditItem;";

                await connection.ExecuteAsync(
                    new CommandDefinition(updateSql, new 
                    { 
                        Result = result, 
                        Observation = obs, 
                        AudioTimestamp = audioTimestamp, 
                        IdAuditItem = existingId.Value 
                    }, transaction: transaction, cancellationToken: ct)
                );
            }
            else
            {
                const string insertSql = @"
                    INSERT INTO sales_service.sales_order_audit_item 
                        (id_audit, id_item, result, observation, audio_timestamp, register)
                    VALUES 
                        (@IdAudit, @IdItem, @Result, @Observation, @AudioTimestamp, NOW());";

                await connection.ExecuteAsync(
                    new CommandDefinition(insertSql, new 
                    { 
                        IdAudit = idAudit, 
                        IdItem = idItem, 
                        Result = result, 
                        Observation = obs, 
                        AudioTimestamp = audioTimestamp 
                    }, transaction: transaction, cancellationToken: ct)
                );
            }

            transaction.Commit();
            return true;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<bool> CloseAuditAsync(long idAudit, string status, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE sales_service.sales_order_audit 
            SET audit_status = @Status, 
                completed_at = NOW() 
            WHERE id_audit = @IdAudit;";

        var rowsAffected = await connection.ExecuteAsync(
            new CommandDefinition(sql, new { Status = status, IdAudit = idAudit }, cancellationToken: ct)
        );

        return rowsAffected > 0;
    }

    public async Task<SalesOrderAudit?> GetAuditByIdAsync(long idAudit, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT 
                id_audit AS IdAudit,
                id_order AS IdOrder,
                id_checklist AS IdChecklist,
                audit_status AS AuditStatus,
                audited_by AS AuditedBy,
                audio_path AS AudioPath,
                notes AS Notes,
                started_at AS StartedAt,
                completed_at AS CompletedAt
            FROM sales_service.sales_order_audit 
            WHERE id_audit = @IdAudit;";

        return await connection.QueryFirstOrDefaultAsync<SalesOrderAudit>(
            new CommandDefinition(sql, new { IdAudit = idAudit }, cancellationToken: ct)
        );
    }
}
