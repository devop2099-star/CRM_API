using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CRM.ApiHub.Domain.DTOs;
using CRM.ApiHub.Domain.Repositories;

namespace CRM.ApiHub.Api.Controllers
{
    [ApiController]
    [Route("api/v1/approvals")]
    [Authorize]
    public class ApprovalController : ControllerBase
    {
        private readonly IApprovalRepository _repository;

        public ApprovalController(IApprovalRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> ProcessApproval([FromBody] ApprovalDto dto)
        {
            // Validamos que el modelo recibido sea correcto
            if (dto == null) return BadRequest("Datos de aprobación inválidos.");

            var id = await _repository.RegisterApprovalAsync(dto);
            
            // Retornamos el resultado del proceso
            return CreatedAtAction(nameof(GetById), new { id }, new { message = "Decisión de aprobación registrada.", id });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            // Endpoint auxiliar para cumplir con el CreatedAtAction
            return Ok(new { id, message = "Consulta de aprobación operativa." });
        }
    }
}