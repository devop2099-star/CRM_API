using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CRM.WebFrontend.Models;

namespace CRM.WebFrontend.Services;

public class BackofficeService : IBackofficeService
{
    private readonly HttpClient _httpClient;

    public BackofficeService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("BackendApi");
    }

    public async Task<IEnumerable<SalesQueueItem>> GetPendingQueueAsync()
    {
        try
        {
            var docs = await _httpClient.GetFromJsonAsync<List<OrderDocumentDto>>("api/backoffice/pending-docs");
            if (docs == null) return Enumerable.Empty<SalesQueueItem>();

            return docs.Select(d => new SalesQueueItem(
                d.IdOrder,
                d.CustomerName ?? "Cliente Desconocido",
                d.Priority ?? "Media",
                d.UploadedAt,
                d.VerificationStatus
            ));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetPendingQueueAsync: {ex.Message}");
            return Enumerable.Empty<SalesQueueItem>();
        }
    }

    public async Task<DocumentVerificationData?> GetVerificationDataAsync(long idOrder)
    {
        try
        {
            // 1. Get documents for this order
            var docs = await _httpClient.GetFromJsonAsync<List<OrderDocumentDto>>($"api/orders/{idOrder}/documents");
            var dniDoc = docs?.FirstOrDefault(d => d.DocumentType.Equals("DNI", StringComparison.OrdinalIgnoreCase));
            if (dniDoc == null) return null;

            // 2. Get order details to get idLead
            var order = await _httpClient.GetFromJsonAsync<SalesOrderDto>($"api/orders/{idOrder}");
            if (order == null) return null;

            // 3. Get lead details
            var lead = await _httpClient.GetFromJsonAsync<LeadDto>($"api/leads/{order.IdLead}");
            if (lead == null) return null;

            return new DocumentVerificationData(
                idOrder,
                dniDoc.FilePath,
                lead.FullName ?? $"{lead.FirstName} {lead.LastName}",
                lead.DocumentNumber ?? "",
                lead.FullName ?? $"{lead.FirstName} {lead.LastName}", // Mock OCR matching form data
                lead.DocumentNumber ?? ""
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetVerificationDataAsync for order {idOrder}: {ex.Message}");
            return null;
        }
    }

    public async Task<IEnumerable<OpenIncidentItem>> GetOpenIncidentsAsync(long idOrder)
    {
        try
        {
            var incidents = await _httpClient.GetFromJsonAsync<List<OrderIncidentDto>>($"api/incidents/order/{idOrder}");
            if (incidents == null) return Enumerable.Empty<OpenIncidentItem>();

            return incidents
                .Where(i => i.IncidentStatus != "Resuelta" && i.IncidentStatus != "RESOLVED")
                .Select(i => new OpenIncidentItem(
                    i.IdOrderIncident,
                    i.CustomDescription ?? i.CustomName ?? "Incidencia sin descripción",
                    i.IncidentStatus ?? "Abierta",
                    i.Register
                ));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetOpenIncidentsAsync for order {idOrder}: {ex.Message}");
            return Enumerable.Empty<OpenIncidentItem>();
        }
    }

    public async Task SubmitVerificationDecisionAsync(long idOrder, string decision, string? observation)
    {
        try
        {
            // 1. Get DNI document ID
            var docs = await _httpClient.GetFromJsonAsync<List<OrderDocumentDto>>($"api/orders/{idOrder}/documents");
            var dniDoc = docs?.FirstOrDefault(d => d.DocumentType.Equals("DNI", StringComparison.OrdinalIgnoreCase));
            if (dniDoc == null) return;

            // Map frontend decision names to backend db values if needed
            string backendStatus = decision.ToUpperInvariant();
            if (backendStatus == "VÁLIDO" || backendStatus == "VALIDO") backendStatus = "VALID";
            if (backendStatus == "INVÁLIDO" || backendStatus == "INVALIDO") backendStatus = "INVALID";
            if (backendStatus == "NO COINCIDE" || backendStatus == "MISMATCH") backendStatus = "MISMATCH";

            // 2. Update document verification status
            var response = await _httpClient.PatchAsJsonAsync($"api/backoffice/documents/{dniDoc.IdDocument}/verify", new
            {
                Status = backendStatus,
                Notes = observation
            });
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in SubmitVerificationDecisionAsync for order {idOrder}: {ex.Message}");
            throw;
        }
    }

    // Local DTOs
    private class OrderDocumentDto
    {
        public long IdDocument { get; set; }
        public long IdOrder { get; set; }
        public string DocumentType { get; set; } = "";
        public string FilePath { get; set; } = "";
        public string VerificationStatus { get; set; } = "";
        public string? CustomerName { get; set; }
        public string? Priority { get; set; }
        public DateTime UploadedAt { get; set; }
    }

    private class SalesOrderDto
    {
        public long IdOrder { get; set; }
        public long IdLead { get; set; }
    }

    private class LeadDto
    {
        public long IdLead { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string? FullName { get; set; }
        public string? DocumentNumber { get; set; }
    }

    private class OrderIncidentDto
    {
        public long IdOrderIncident { get; set; }
        public string? CustomName { get; set; }
        public string? CustomDescription { get; set; }
        public string? IncidentStatus { get; set; }
        public DateTime Register { get; set; }
    }
}
