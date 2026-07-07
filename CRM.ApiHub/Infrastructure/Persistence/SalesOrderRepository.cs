using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CRM.ApiHub.Domain.Entities;
using CRM.ApiHub.Domain.Repositories;
using Dapper;

namespace CRM.ApiHub.Infrastructure.Persistence;

public class SalesOrderRepository : ISalesOrderRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public SalesOrderRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<SalesOrder>> GetByFiltersAsync(
        long? userId,
        long? statusId,
        long? campaignId,
        DateTime? dateFrom,
        DateTime? dateTo,
        CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = new StringBuilder("SELECT * FROM sales_service.sales_order WHERE 1=1");
        var parameters = new DynamicParameters();

        if (userId.HasValue)
        {
            sql.Append(" AND id_user = @UserId");
            parameters.Add("UserId", userId.Value);
        }

        if (statusId.HasValue)
        {
            sql.Append(" AND id_status = @StatusId");
            parameters.Add("StatusId", statusId.Value);
        }

        if (campaignId.HasValue)
        {
            sql.Append(" AND id_cmpg = @CampaignId");
            parameters.Add("CampaignId", campaignId.Value);
        }

        if (dateFrom.HasValue)
        {
            sql.Append(" AND sales_date >= @DateFrom");
            parameters.Add("DateFrom", dateFrom.Value);
        }

        if (dateTo.HasValue)
        {
            sql.Append(" AND sales_date <= @DateTo");
            parameters.Add("DateTo", dateTo.Value);
        }

        sql.Append(" ORDER BY sales_date DESC;");

        return await connection.QueryAsync<SalesOrder>(
            new CommandDefinition(sql.ToString(), parameters, cancellationToken: ct)
        );
    }

    public async Task<SalesOrder?> GetByIdAsync(long idOrder, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "SELECT * FROM sales_service.sales_order WHERE id_order = @IdOrder;";

        return await connection.QueryFirstOrDefaultAsync<SalesOrder>(
            new CommandDefinition(sql, new { IdOrder = idOrder }, cancellationToken: ct)
        );
    }

    public async Task<long> CreateAsync(SalesOrder order, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO sales_service.sales_order (
                id_lead, id_cmpg, id_user, owner_user_id, custody_user_id,
                id_status, id_substatus, currency_code, commission_currency,
                status, sales_date, total_products, total_value, is_alternate,
                register, last_update
            )
            VALUES (
                @IdLead, @IdCmpg, @IdUser, @OwnerUserId, @CustodyUserId,
                @IdStatus, @IdSubstatus, @CurrencyCode, @CommissionCurrency,
                @Status, @SalesDate, @TotalProducts, @TotalValue, @IsAlternate,
                @Register, @LastUpdate
            )
            RETURNING id_order;";

        return await connection.ExecuteScalarAsync<long>(
            new CommandDefinition(sql, order, cancellationToken: ct)
        );
    }

    public async Task<bool> UpdateStatusAsync(
        long idOrder,
        long toStatusId,
        long? toSubstatusId,
        string? comment,
        long actorId,
        bool isBulk,
        CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        if (connection.State != ConnectionState.Open)
        {
            connection.Open();
        }

        using var transaction = connection.BeginTransaction();
        try
        {
            await connection.ExecuteAsync(
                "SELECT set_config('app.current_user_id', @ActorId, true);",
                new { ActorId = actorId.ToString() },
                transaction: transaction
            );

            const string selectSql = "SELECT id_status, id_substatus FROM sales_service.sales_order WHERE id_order = @IdOrder FOR UPDATE;";
            var current = await connection.QueryFirstOrDefaultAsync<dynamic>(
                new CommandDefinition(selectSql, new { IdOrder = idOrder }, transaction: transaction, cancellationToken: ct)
            );

            if (current == null)
            {
                transaction.Rollback();
                return false;
            }

            long? fromStatusId = current.id_status != null ? (long?)current.id_status : null;
            long? fromSubstatusId = current.id_substatus != null ? (long?)current.id_substatus : null;

            const string updateSql = @"
                UPDATE sales_service.sales_order 
                SET id_status = @ToStatusId, id_substatus = @ToSubstatusId, last_update = NOW()
                WHERE id_order = @IdOrder;";

            await connection.ExecuteAsync(
                new CommandDefinition(updateSql, new { ToStatusId = toStatusId, ToSubstatusId = toSubstatusId, IdOrder = idOrder }, transaction: transaction, cancellationToken: ct)
            );

            if (fromStatusId != toStatusId || fromSubstatusId != toSubstatusId)
            {
                const string getHistoryIdSql = @"
                    SELECT id_history 
                    FROM sales_service.sales_order_status_history 
                    WHERE id_order = @IdOrder 
                    ORDER BY id_history DESC 
                    LIMIT 1;";

                var historyId = await connection.ExecuteScalarAsync<long?>(
                    new CommandDefinition(getHistoryIdSql, new { IdOrder = idOrder }, transaction: transaction, cancellationToken: ct)
                );

                if (historyId.HasValue)
                {
                    const string updateHistorySql = @"
                        UPDATE sales_service.sales_order_status_history 
                        SET comment = @Comment, is_bulk = @IsBulk
                        WHERE id_history = @HistoryId;";

                    await connection.ExecuteAsync(
                        new CommandDefinition(updateHistorySql, new { Comment = comment, IsBulk = isBulk, HistoryId = historyId.Value }, transaction: transaction, cancellationToken: ct)
                    );
                }
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
}