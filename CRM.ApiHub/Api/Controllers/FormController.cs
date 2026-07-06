using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CRM.ApiHub.Domain.Entities;
using CRM.ApiHub.Domain.Repositories;
using CRM.ApiHub.Application.Interfaces; 

namespace CRM.ApiHub.Api.Controllers;

[ApiController]
[Route("api/v1/forms")]
[Authorize] 
public class FormController : ControllerBase
{
    private readonly IFormRepository _formRepository;
    private readonly IOrderDataRepository _orderDataRepository;
    private readonly INotificationService _notificationService; 

    public FormController(IFormRepository formRepository, IOrderDataRepository orderDataRepository, INotificationService notificationService) 
    {
        _formRepository = formRepository;
        _orderDataRepository = orderDataRepository;
        _notificationService = notificationService; 
    }

    [HttpGet("campaign/{idCmpg}/stage/{idStage}")]
    public async Task<IActionResult> GetTemplates(long idCmpg, long idStage)
    {
        var templates = await _formRepository.GetTemplatesByCampaignStageAsync(idCmpg, idStage);
        return Ok(templates);
    }

    [HttpGet("{idForm}/fields")]
    public async Task<IActionResult> GetFields(long idForm)
    {
        var fields = await _formRepository.GetFieldsByTemplateAsync(idForm);
        return Ok(fields);
    }

    [HttpGet("order/{idOrder}/data")]
    public async Task<IActionResult> GetOrderData(long idOrder)
    {
        var data = await _orderDataRepository.GetByOrderAsync(idOrder);
        return Ok(data);
    }

    [HttpPost("order/{idOrder}/template/{idForm}")]
    public async Task<IActionResult> SaveData(long idOrder, long idForm, [FromBody] IEnumerable<OrderData> fields)
    {
        if (fields == null || !fields.Any())
            return BadRequest("La lista de campos no puede estar vacía.");

        await _orderDataRepository.SaveOrderDataAsync(idOrder, idForm, fields);
        return Ok(new { message = "Datos del formulario guardados exitosamente." });
    }

    [HttpPut("data/{idData}/status")]
    public async Task<IActionResult> UpdateStatus(long idData, [FromQuery] string status, [FromQuery] long validatedBy)
    {
        if (string.IsNullOrWhiteSpace(status))
            return BadRequest("El estado es requerido.");

        await _orderDataRepository.UpdateFieldStatusAsync(idData, status, validatedBy);

        await _notificationService.SendNotificationAsync(
            userId: validatedBy, 
            title: "Cambio de Estado",
            message: $"El registro {idData} ha cambiado al estado: {status}.",
            module: "Ventas"
        );

        return Ok(new { message = "Estado actualizado correctamente." });
    }
}