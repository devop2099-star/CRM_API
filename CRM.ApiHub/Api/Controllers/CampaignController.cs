using System;
using System.Threading.Tasks;
using CRM.ApiHub.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.ApiHub.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/campaigns")]
public class CampaignController : ControllerBase
{
    private readonly ICampaignRepository _campaignRepository;
    private readonly ICatalogRepository _catalogRepository;

    public CampaignController(ICampaignRepository campaignRepository, ICatalogRepository catalogRepository)
    {
        _campaignRepository = campaignRepository;
        _catalogRepository = catalogRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetCampaigns()
    {
        try
        {
            var campaigns = await _campaignRepository.GetAllActiveAsync();
            return Ok(campaigns);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener las campañas.", details = ex.Message });
        }
    }

    [HttpGet("{id}/statuses")]
    public async Task<IActionResult> GetOrderStatuses(int id)
    {
        try
        {
            // Normally you might filter statuses by campaign. 
            // For now, we return all active statuses from the catalog.
            var statuses = await _catalogRepository.GetOrderStatusesAsync();
            return Ok(statuses);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener los estados.", details = ex.Message });
        }
    }
}
