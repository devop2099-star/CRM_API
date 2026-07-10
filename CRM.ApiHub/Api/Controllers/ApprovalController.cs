using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CRM.ApiHub.Domain.DTOs;
using CRM.ApiHub.Domain.Repositories;
using CRM.ApiHub.Application.Interfaces;
using System.Security.Claims;

namespace CRM.ApiHub.Api.Controllers;

public class ApprovalRequestDto
{
    public long AuthorizedBy { get; set; }
    public string Comments { get; set; } = null!;
    public bool IsApproved { get; set; }
}

public class ApprovalPatchDto
{
    public string Status { get; set; } = null!; // APPROVED or REJECTED
    public string Comments { get; set; } = null!;
    public long AuthorizedBy { get; set; }
}

[ApiController]
[Authorize]
public class ApprovalController : ControllerBase
{
    private readonly IApprovalRepository _repository;
    private readonly ISalesOrderRepository _salesOrderRepository;
    private readonly INotificationService _notificationService;

    public ApprovalController(
        IApprovalRepository repository,
        ISalesOrderRepository salesOrderRepository,
        INotificationService notificationService)
    {
        _repository = repository;
        _salesOrderRepository = salesOrderRepository;
        _notificationService = notificationService;
    }

    [HttpPost("api/orders/{id:long}/approvals")]
    [Authorize(Roles = "SUPERVISOR")]
    public async Task<IActionResult> ProcessApproval(long id, [FromBody] ApprovalRequestDto dto)
    {
        if (dto == null) return BadRequest("Datos de aprobación inválidos.");

        var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? User.FindFirst("roles")?.Value;
        if (userRole != "SUPERVISOR")
        {
            return StatusCode(403, new { message = "Acceso denegado: Solo el rol SUPERVISOR puede registrar aprobaciones." });
        }

        var approvalDto = new ApprovalDto
        {
            IdOrder = id,
            AuthorizedBy = dto.AuthorizedBy,
            Comments = dto.Comments,
            IsApproved = dto.IsApproved
        };

        var approvalId = await _repository.RegisterApprovalAsync(approvalDto);

        // Emit internal notification on approval
        if (dto.IsApproved)
        {
            var order = await _salesOrderRepository.GetByIdAsync(id);
            if (order != null)
            {
                await _notificationService.SendNotificationAsync(
                    userId: order.IdUser,
                    title: "Orden Aprobada",
                    message: $"Tu orden #{id} ha sido aprobada por el supervisor.",
                    module: "Approvals",
                    actionData: approvalId.ToString()
                );
            }
        }

        return CreatedAtAction(nameof(GetById), new { id = approvalId }, new { message = "Decisión de aprobación registrada.", id = approvalId });
    }

    [HttpPatch("api/approvals/{id:long}")]
    [Authorize(Roles = "SUPERVISOR")]
    public async Task<IActionResult> UpdateApproval(long id, [FromBody] ApprovalPatchDto dto)
    {
        if (dto == null) return BadRequest("Datos de modificación inválidos.");

        var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? User.FindFirst("roles")?.Value;
        if (userRole != "SUPERVISOR")
        {
            return StatusCode(403, new { message = "Acceso denegado: Solo el rol SUPERVISOR puede modificar aprobaciones." });
        }

        var success = await _repository.UpdateApprovalAsync(id, dto.Status, dto.Comments, dto.AuthorizedBy);
        if (!success)
        {
            return NotFound(new { message = "Aprobación no encontrada." });
        }

        // Emit internal notification on approval
        if (dto.Status == "APPROVED")
        {
            var approval = await _repository.GetApprovalByIdAsync(id);
            if (approval != null)
            {
                var order = await _salesOrderRepository.GetByIdAsync(approval.IdOrder);
                if (order != null)
                {
                    await _notificationService.SendNotificationAsync(
                        userId: order.IdUser,
                        title: "Orden Aprobada",
                        message: $"Tu orden #{approval.IdOrder} ha sido aprobada por el supervisor.",
                        module: "Approvals",
                        actionData: id.ToString()
                    );
                }
            }
        }

        return Ok(new { message = "Aprobación actualizada correctamente.", id = id });
    }

    [HttpGet("api/approvals/{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var approval = await _repository.GetApprovalByIdAsync(id);
        if (approval == null)
        {
            return NotFound(new { message = "Aprobación no encontrada." });
        }
        return Ok(approval);
    }
}