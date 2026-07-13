using CRM.WebFrontend.Models;

namespace CRM.WebFrontend.Services;

public class MockBackofficeService : IBackofficeService
{
    public async Task<IEnumerable<SalesQueueItem>> GetPendingQueueAsync()
    {
        await Task.Delay(500); 
        return new List<SalesQueueItem>
        {
            new(101, "Carlos Mendoza", "Alta", DateTime.Now.AddMinutes(-15), "Pendiente DNI"),
            new(102, "Ana Lucía Rojas", "Media", DateTime.Now.AddMinutes(-45), "Pendiente DNI"),
            new(103, "Jorge Pérez", "Baja", DateTime.Now.AddHours(-2), "Pendiente DNI")
        };
    }

    public async Task<DocumentVerificationData?> GetVerificationDataAsync(long idOrder)
    {
        await Task.Delay(300);
        return new DocumentVerificationData(
            idOrder,
            "https://via.placeholder.com/600x400.png?text=DNI+Frontal+Escaneado",
            "Carlos Mendoza",
            "74859612",
            "Carlos Mendoza",
            "74859612"
        );
    }

    public async Task<IEnumerable<OpenIncidentItem>> GetOpenIncidentsAsync(long idOrder)
    {
        await Task.Delay(200);
        return new List<OpenIncidentItem>
        {
            new(501, "Error de validación en buró de crédito", "Abierta", DateTime.Now.AddDays(-1))
        };
    }

    public async Task SubmitVerificationDecisionAsync(long idOrder, string decision, string? observation)
    {
        await Task.Delay(400);
        Console.WriteLine($"Orden {idOrder} marcada como: {decision}. Obs: {observation}");
    }

    public async Task<IEnumerable<BackofficeIncidentDto>> GetIncidentsAsync(string? role = "BAC", string? status = "OPEN")
    {
        await Task.Delay(100);
        return new List<BackofficeIncidentDto>
        {
            new(501, 101, 1, "Incidencia de Buró", "Detalle de Buró", "OPEN", "BAC", DateTime.UtcNow.AddHours(2), DateTime.UtcNow)
        };
    }

    public async Task<IEnumerable<KbArticleSuggestionDto>> GetKbSuggestionsAsync(long idIncident)
    {
        await Task.Delay(100);
        return new List<KbArticleSuggestionDto>
        {
            new(1, "Cómo validar Buró", "Guía paso a paso para validación manual de buró.", "como-validar-buro")
        };
    }

    public async Task AddIncidentResponseAsync(long idIncident, string responseText, string responseType, long respondedBy)
    {
        await Task.Delay(100);
    }

    public async Task ResolveIncidentAsync(long idIncident, string notes, long resolvedBy)
    {
        await Task.Delay(100);
    }
}