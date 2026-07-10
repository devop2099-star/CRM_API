using Microsoft.AspNetCore.Mvc;
using CRM.ApiHub.Domain.Repositories;
using CRM.ApiHub.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using CRM.ApiHub.Api.Filters;
using System.Security.Claims;

namespace CRM.ApiHub.Api.Controllers;

// DTOs auxiliares para tipado fuerte y correcta recepción de JSON
public record CallLogRequest(string CallLog);
public record AssignRequest(int ToUserId, string Context);
public record ConvertRequest(int UserId);
public record PreSaleCreateDto(
    long IdCmpg,
    string? Phone,
    string? Operator,
    string? FirstName,
    string? LastName,
    string? Address,
    string? Province,
    string? CoverageStatus,
    long IdStatus,
    long OwnerUserId,
    long CurrentUserId,
    string? Notes
);

[Authorize]
[ApiController]
[Route("presales")]
public class PreSaleController : ControllerBase
{
    private readonly IPreSaleRepository _repository;

    public PreSaleController(IPreSaleRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetByUser([FromQuery] int userId)
    {
        var preSales = await _repository.GetByUserAsync(userId);
        return Ok(preSales);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PreSaleCreateDto dto)
    {
        var preSale = new LeadPreSale
        {
            IdCmpg = dto.IdCmpg,
            Phone = dto.Phone,
            Operator = dto.Operator,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Address = dto.Address,
            Province = dto.Province,
            CoverageStatus = dto.CoverageStatus,
            IdStatus = dto.IdStatus,
            OwnerUserId = dto.OwnerUserId,
            CurrentUserId = dto.CurrentUserId,
            Notes = dto.Notes
        };

        var id = await _repository.CreateAsync(preSale);
        return CreatedAtAction(nameof(GetByUser), new { userId = preSale.CurrentUserId }, new { id });
    }

    [HttpPost("{id}/calls")]
    public async Task<IActionResult> AddCallLog(int id, [FromBody] CallLogRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        long userId = 1;
        if (userIdClaim != null && long.TryParse(userIdClaim.Value, out long parsedId))
        {
            userId = parsedId;
        }

        var result = await _repository.AddCallLogAsync(id, request.CallLog, userId);
        if (!result) return BadRequest(new { message = "No se pudo registrar el log de la llamada." });
        return Ok(new { message = "Log de llamada registrado con éxito." });
    }

    [HttpPost("{id}/assign")]
    public async Task<IActionResult> Assign(int id, [FromBody] AssignRequest request)
    {
        var result = await _repository.AssignAsync(id, request.ToUserId, request.Context);
        if (!result) return BadRequest(new { message = "No se pudo reasignar la pre-venta." });
        return Ok(new { message = "Pre-venta reasignada con éxito." });
    }

    [HttpPost("{id}/convert")]
    public async Task<IActionResult> Convert(int id, [FromBody] ConvertRequest request)
    {
        var result = await _repository.ConvertAsync(id, new { UserId = request.UserId });
        if (!result) return BadRequest(new { message = "No se pudo convertír la pre-venta." });
        return Ok(new { message = "Pre-venta convertida a cliente con éxito." });
    }
}