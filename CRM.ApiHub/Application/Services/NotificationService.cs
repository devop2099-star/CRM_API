using CRM.ApiHub.Application.Interfaces;
using CRM.ApiHub.Domain.Repositories;

namespace CRM.ApiHub.Application.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task SendNotificationAsync(long userId, string title, string message, string? module = null, string? actionData = null)
    {
        await _notificationRepository.CreateAsync(userId, title, message, module, actionData);
    }
}