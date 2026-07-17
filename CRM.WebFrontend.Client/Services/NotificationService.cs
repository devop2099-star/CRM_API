using System.Net.Http.Json;
using CRM.WebFrontend.Client.Models;

namespace CRM.WebFrontend.Client.Services;

public class NotificationService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public NotificationService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<List<NotificationDto>> GetUnreadAsync(long userId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("BackendApi");
            var result = await client.GetFromJsonAsync<List<NotificationDto>>($"/api/notifications?userId={userId}");
            return result ?? new List<NotificationDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[NotificationService] Error fetching notifications: {ex.Message}");
            return new List<NotificationDto>();
        }
    }

    public async Task MarkAsReadAsync(int idAlert)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("BackendApi");
            await client.PatchAsync($"/api/notifications/{idAlert}/read", null);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[NotificationService] Error marking as read: {ex.Message}");
        }
    }

    public async Task MarkAllAsReadAsync(long userId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("BackendApi");
            await client.PostAsync($"/api/notifications/read-all?userId={userId}", null);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[NotificationService] Error marking all as read: {ex.Message}");
        }
    }
}
