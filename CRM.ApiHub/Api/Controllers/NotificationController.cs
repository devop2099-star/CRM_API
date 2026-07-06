using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CRM.ApiHub.Domain.Repositories;

namespace CRM.ApiHub.Api.Controllers;

[ApiController]
[Route("api/v1/notifications")]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationController(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetUnread([FromQuery] long userId)
    {
        var notifications = await _notificationRepository.GetUnreadAsync(userId);
        return Ok(notifications);
    }

    [HttpPatch("{id}/read")] 
    public async Task<IActionResult> MarkAsRead(int id)
    {
        await _notificationRepository.MarkReadAsync(id);
        return Ok(new { message = "Notificación marcada como leída." });
    }

    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllAsRead([FromQuery] long userId)
    {
        await _notificationRepository.MarkAllReadAsync(userId);
        return Ok(new { message = "Todas las notificaciones marcadas como leídas." });
    }
}