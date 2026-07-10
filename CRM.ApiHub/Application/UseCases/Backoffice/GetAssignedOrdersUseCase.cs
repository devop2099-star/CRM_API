using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CRM.ApiHub.Domain.Entities;
using CRM.ApiHub.Domain.Repositories;

namespace CRM.ApiHub.Application.UseCases.Backoffice;

public class GetAssignedOrdersUseCase
{
    private readonly IBackofficeRepository _repository;

    public GetAssignedOrdersUseCase(IBackofficeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<SalesOrder>> ExecuteAsync(
        long backofficeId,
        long? userId,
        long? statusId,
        long? campaignId,
        DateTime? dateFrom,
        DateTime? dateTo,
        CancellationToken ct = default)
    {
        return await _repository.GetAssignedOrdersAsync(backofficeId, userId, statusId, campaignId, dateFrom, dateTo, ct);
    }
}
