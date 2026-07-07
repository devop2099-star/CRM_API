using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CRM.ApiHub.Domain.Repositories; 
using CRM.ApiHub.Domain.DTOs;         
using CRM.ApiHub.Domain.Entities;     

[ApiController]
[Route("api/v1/alternate-profiles")]
[Authorize]
public class AlternateProfileController : ControllerBase
{
    private readonly IAlternateProfileRepository _repository;

    public AlternateProfileController(IAlternateProfileRepository repository)
    {
        _repository = repository;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AlternateProfileDto dto)
    {
        // Mapea tu DTO a tu entidad de dominio
        var profile = new AlternateProfile {
            IdOrder = dto.IdOrder,
            AlternateType = dto.AlternateType,
            AlternateData = dto.AlternateData,
            OriginalData = dto.OriginalData,
            Reason = dto.Reason,
            CreatedBy = dto.CreatedBy
        };

        var id = await _repository.CreateAsync(profile);
        return CreatedAtAction(nameof(GetById), new { id }, new { message = "Ficha alterna creada.", id });
    }

[HttpGet("{id}")]
public async Task<IActionResult> GetById(long id)
{
    // Por ahora, esto cumple el contrato para que CreatedAtAction funcione
    return Ok(new { id, message = "Endpoint de lectura operativo" });
}
}