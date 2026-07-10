using CRM.WebFrontend.Models;

namespace CRM.WebFrontend.Services;

public interface IBackofficeService
{
    Task<IEnumerable<SalesQueueItem>> GetPendingQueueAsync();
    Task<DocumentVerificationData?> GetVerificationDataAsync(long idOrder);
    Task<IEnumerable<OpenIncidentItem>> GetOpenIncidentsAsync(long idOrder);
    Task SubmitVerificationDecisionAsync(long idOrder, string decision, string? observation);
}