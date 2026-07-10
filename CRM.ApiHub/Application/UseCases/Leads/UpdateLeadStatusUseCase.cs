using System.Threading;
using System.Threading.Tasks;
using CRM.ApiHub.Application.DTOs;
using CRM.ApiHub.Domain.Repositories;
using CRM.ApiHub.Application.Interfaces;

namespace CRM.ApiHub.Application.UseCases.Leads;

public class UpdateLeadStatusUseCase
{
    private readonly ILeadRepository _leadRepository;
    private readonly INotificationService _notificationService;
    private readonly ISalesOrderRepository _salesOrderRepository;
    private readonly IIncidentRepository _incidentRepository;

    public UpdateLeadStatusUseCase(
        ILeadRepository leadRepository,
        INotificationService notificationService,
        ISalesOrderRepository salesOrderRepository,
        IIncidentRepository incidentRepository)
    {
        _leadRepository = leadRepository;
        _notificationService = notificationService;
        _salesOrderRepository = salesOrderRepository;
        _incidentRepository = incidentRepository;
    }

    public async Task<bool> ExecuteAsync(long idLead, LeadUpdateStatusDto dto, long actorId, CancellationToken ct = default)
    {
        var success = await _leadRepository.UpdateStatusAsync(idLead, dto.IdStatus, dto.Comment, actorId, ct);
        if (success)
        {
            var lead = await _leadRepository.GetByIdAsync(idLead, ct);
            if (lead != null)
            {
                long recipientId = lead.AssignedUserId ?? lead.OwnerUserId ?? actorId;
                await _notificationService.SendNotificationAsync(
                    userId: recipientId,
                    title: "Estado de Lead Actualizado",
                    message: $"El lead '{lead.FirstName} {lead.LastName}' ha cambiado al estado {dto.IdStatus}.",
                    module: "Lead",
                    actionData: idLead.ToString()
                );

                // Create incident for testing
                var order = await _salesOrderRepository.GetByLeadIdAsync(idLead, ct);
                if (order == null)
                {
                    // Create a temporary sales order for this lead to allow incident creation
                    var newOrder = new CRM.ApiHub.Domain.Entities.SalesOrder
                    {
                        IdLead = idLead,
                        IdCmpg = lead.IdCmpg ?? 1,
                        IdUser = actorId,
                        OwnerUserId = actorId,
                        CustodyUserId = actorId,
                        IdStatus = 1, // PENDING_VALIDATION
                        CurrencyCode = "EUR",
                        CommissionCurrency = "PEN",
                        Status = "PENDING_VALIDATION",
                        SalesDate = System.DateTime.UtcNow,
                        TotalProducts = 0,
                        TotalValue = 0,
                        IsAlternate = false,
                        Register = System.DateTime.UtcNow,
                        LastUpdate = System.DateTime.UtcNow
                    };
                    long newOrderId = await _salesOrderRepository.CreateAsync(newOrder, ct);
                    order = await _salesOrderRepository.GetByIdAsync(newOrderId, ct);
                }

                if (order != null)
                {
                    var testIncident = new CRM.ApiHub.Domain.Entities.OrderIncident
                    {
                        IdOrder = order.IdOrder,
                        IdIncident = 1, // M6 - Titular no coincide
                        CustomName = $"Incidencia por cambio de estado de Lead a {dto.IdStatus}",
                        CustomDescription = $"Se generó automáticamente esta incidencia debido al cambio de estado del lead a {dto.IdStatus}. Comentario: {dto.Comment}",
                        IncidentStatus = "OPEN",
                        DetectedBy = actorId,
                        AssignedToRole = "SUPPORT",
                        Register = System.DateTime.UtcNow
                    };
                    await _incidentRepository.CreateAsync(testIncident);
                }
            }
        }
        return success;
    }
}
