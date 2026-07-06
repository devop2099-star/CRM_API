using System.Threading;
using System.Threading.Tasks;
using CRM.ApiHub.Application.DTOs;
using CRM.ApiHub.Domain.Repositories;

namespace CRM.ApiHub.Application.UseCases.SalesOrders;

public class UpdateSalesOrderStatusUseCase
{
    private readonly ISalesOrderRepository _salesOrderRepository;

    public UpdateSalesOrderStatusUseCase(ISalesOrderRepository salesOrderRepository)
    {
        _salesOrderRepository = salesOrderRepository;
    }

    public async Task<bool> ExecuteAsync(long idOrder, SalesOrderUpdateStatusDto dto, long actorId, CancellationToken ct = default)
    {
        return await _salesOrderRepository.UpdateStatusAsync(
            idOrder,
            dto.ToStatusId,
            dto.ToSubstatusId,
            dto.Comment,
            actorId,
            dto.IsBulk,
            ct
        );
    }
}
