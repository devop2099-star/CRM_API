using System.Threading;
using System.Threading.Tasks;
using CRM.ApiHub.Application.DTOs;
using CRM.ApiHub.Domain.Repositories;
using CRM.ApiHub.Application.Interfaces;

namespace CRM.ApiHub.Application.UseCases.SalesOrders;

public class UpdateSalesOrderStatusUseCase
{
    private readonly ISalesOrderRepository _salesOrderRepository;
    private readonly INotificationService _notificationService;

    public UpdateSalesOrderStatusUseCase(
        ISalesOrderRepository salesOrderRepository,
        INotificationService notificationService)
    {
        _salesOrderRepository = salesOrderRepository;
        _notificationService = notificationService;
    }

    public async Task<bool> ExecuteAsync(long idOrder, SalesOrderUpdateStatusDto dto, long actorId, CancellationToken ct = default)
    {
        var success = await _salesOrderRepository.UpdateStatusAsync(
            idOrder,
            dto.ToStatusId,
            dto.ToSubstatusId,
            dto.Comment,
            actorId,
            dto.IsBulk,
            ct
        );

        if (success)
        {
            var order = await _salesOrderRepository.GetByIdAsync(idOrder, ct);
            if (order != null)
            {
                await _notificationService.SendNotificationAsync(
                    userId: order.IdUser,
                    title: "Estado de Orden Actualizado",
                    message: $"El estado de tu orden #{idOrder} ha cambiado al estado {dto.ToStatusId}.",
                    module: "SalesOrder",
                    actionData: idOrder.ToString()
                );
            }
        }

        return success;
    }
}
