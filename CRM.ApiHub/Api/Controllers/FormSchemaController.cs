using System;
using System.Threading;
using System.Threading.Tasks;
using CRM.ApiHub.Application.UseCases.SalesOrders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.ApiHub.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/forms")]
public class FormSchemaController : ControllerBase
{
    private readonly GetFormSchemaUseCase _getFormSchemaUseCase;

    public FormSchemaController(GetFormSchemaUseCase getFormSchemaUseCase)
    {
        _getFormSchemaUseCase = getFormSchemaUseCase;
    }

    [HttpGet("schema")]
    public async Task<IActionResult> GetSchema([FromQuery] long campaignId, [FromQuery] long statusId, CancellationToken ct)
    {
        try
        {
            var schema = await _getFormSchemaUseCase.ExecuteAsync(campaignId, statusId, ct);
            if (schema == null)
            {
                return NotFound(new { message = "No se encontró plantilla de formulario para esta campaña y estado." });
            }
            return Ok(schema);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener el esquema del formulario.", details = ex.Message });
        }
    }
}
