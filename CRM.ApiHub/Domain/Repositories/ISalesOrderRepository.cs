using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CRM.ApiHub.Domain.Entities;

namespace CRM.ApiHub.Domain.Repositories;

public interface ISalesOrderRepository
{
    Task<IEnumerable<SalesOrder>> GetByFiltersAsync(
        long? userId,
        long? statusId,
        long? campaignId,
        DateTime? dateFrom,
        DateTime? dateTo,
        CancellationToken ct = default);

    Task<SalesOrder?> GetByIdAsync(long idOrder, CancellationToken ct = default);

    Task<long> CreateAsync(SalesOrder order, CancellationToken ct = default);

    Task<bool> UpdateStatusAsync(
        long idOrder,
        long toStatusId,
        long? toSubstatusId,
        string? comment,
        long actorId,
        bool isBulk,
        CancellationToken ct = default);
}
