using CRM.ApiHub.Domain.Entities;

namespace CRM.ApiHub.Domain.Repositories;

public interface INotificationRepository
{
    Task CreateAsync(long userId, string title, string message, string? module, string? actionData);
    Task<IEnumerable<UserAlert>> GetUnreadAsync(long userId);
    Task MarkReadAsync(int idAlert);
    Task MarkAllReadAsync(long userId); 
}