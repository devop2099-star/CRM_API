using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CRM.ApiHub.Domain.Repositories;
using CRM.ApiHub.Application.DTOs;

namespace CRM.ApiHub.Application.UseCases.SalesOrders;

public class GetSalesOrdersUseCase
{
    private readonly ISalesOrderRepository _salesOrderRepository;

    public GetSalesOrdersUseCase(ISalesOrderRepository salesOrderRepository)
    {
        _salesOrderRepository = salesOrderRepository;
    }

    public async Task<IEnumerable<SalesOrderListDto>> ExecuteAsync(
        long? userId,
        long? statusId,
        long? campaignId,
        DateTime? dateFrom,
        DateTime? dateTo,
        CancellationToken ct = default)
    {
        return await _salesOrderRepository.GetByFiltersAsync(userId, statusId, campaignId, dateFrom, dateTo, ct);
    }
}
