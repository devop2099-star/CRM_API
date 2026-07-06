using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CRM.ApiHub.Domain.Entities;
using CRM.ApiHub.Domain.Repositories;

namespace CRM.ApiHub.Api.Controllers;

[ApiController]
[Route("api/v1/incidents")]
[Authorize]
public class IncidentController : ControllerBase
{
    private readonly IIncidentRepository _incidentRepository;

    public IncidentController(IIncidentRepository incidentRepository)
    {
        _incidentRepository = incidentRepository;
    }

    [HttpGet("catalog")]
    public async Task<IActionResult> GetCatalog([FromQuery] long idCmpg, [FromQuery] long idStatus)
    {
        var catalog = await _incidentRepository.GetCatalogAsync(idCmpg, idStatus);
        return Ok(catalog);
    }

    [HttpGet("order/{idOrder}")]
    public async Task<IActionResult> GetByOrder(long idOrder)
    {
        var incidents = await _incidentRepository.GetByOrderAsync(idOrder);
        return Ok(incidents);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] OrderIncident incident)
    {
        var newIncidentId = await _incidentRepository.CreateAsync(incident);

        var suggestedArticles = await _incidentRepository.GetKbSuggestionsAsync(incident.IdIncident);

        return Created($"/api/v1/incidents/order/{incident.IdOrder}", new 
        { 
            message = "Incidencia creada correctamente.",
            idOrderIncident = newIncidentId,
            kbSuggestions = suggestedArticles 
        });
    }

    [HttpPost("{id}/responses")]
    public async Task<IActionResult> CreateResponse(long id, [FromQuery] string responseText, [FromQuery] string responseType, [FromQuery] long respondedBy)
    {
        await _incidentRepository.CreateResponseAsync(id, responseText, responseType, respondedBy);
        return Ok(new { message = "Respuesta registrada." });
    }

    [HttpPatch("{id}/resolve")]
    public async Task<IActionResult> Resolve(long id, [FromQuery] string notes, [FromQuery] long resolvedBy)
    {
        await _incidentRepository.ResolveAsync(id, notes, resolvedBy);
        return Ok(new { message = "Incidencia resuelta correctamente." });
    }
}