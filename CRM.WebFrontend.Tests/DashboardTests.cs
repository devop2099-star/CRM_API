using Bunit;
using Bunit.TestDoubles;
using CRM.WebFrontend.Components.Pages;
using CRM.WebFrontend.Models;
using CRM.WebFrontend.Services;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CRM.WebFrontend.Tests;

public class DashboardTests
{
    [Fact]
    public void Dashboard_ShouldRenderQueueAndLoadDocumentOnSelection()
    {
        // Arrange
        using var ctx = new TestContext();
        
        // Setup mock authentication for BACKOFFICE role
        var authContext = ctx.AddTestAuthorization();
        authContext.SetRoles("BACKOFFICE");
        authContext.SetAuthorized("TestUser");

        // Register MudBlazor services because of global import
        ctx.Services.AddMudServices();

        // Create fake Backoffice service
        var fakeService = new FakeBackofficeService();
        ctx.Services.AddSingleton<IBackofficeService>(fakeService);

        // Act - Render the Dashboard component
        var cut = ctx.RenderComponent<Dashboard>();

        // Assert - Queue is rendered
        var queueItems = cut.FindAll(".queue-item");
        Assert.Equal(3, queueItems.Count);

        // Verify the first item has correct text
        var firstItem = queueItems[0];
        Assert.Contains("Carlos Mendoza", firstItem.OuterHtml);
        Assert.Contains("Ord. #101", firstItem.OuterHtml);

        // Verify that before selection, the empty state is displayed
        var emptyState = cut.Find(".premium-panel.d-flex.flex-column.align-items-center");
        Assert.Contains("Ningún documento seleccionado", emptyState.InnerHtml);

        // Select the first item by clicking it
        firstItem.Click();

        // Assert - Verification data is rendered
        // 1. Verify image URL is rendered correctly
        var img = cut.Find(".doc-viewer img");
        Assert.Equal("/api/documents/101/download", img.GetAttribute("src"));

        // 2. Verify form data contrast
        var formName = cut.FindAll(".data-value")[0];
        Assert.Equal("Carlos Mendoza", formName.TextContent);
        
        var formDocNum = cut.FindAll(".data-value")[1];
        Assert.Equal("74859612", formDocNum.TextContent);

        // 3. Verify OCR data contrast
        var ocrName = cut.FindAll(".data-value")[2];
        Assert.Equal("Carlos Mendoza OCR", ocrName.TextContent);

        var ocrDocNum = cut.FindAll(".data-value")[3];
        Assert.Equal("74859612-OCR", ocrDocNum.TextContent);

        // 4. Verify Incidents are rendered
        var alertPanel = cut.Find(".alert-panel");
        Assert.Contains("Incidencias Abiertas Detectadas", alertPanel.InnerHtml);
        Assert.Contains("Error de validación en buró", alertPanel.InnerHtml);

        // Act - Write observation and submit decision "Válido"
        var input = cut.Find("input.premium-input");
        input.Change("Todo correcto");

        var validoBtn = cut.FindAll(".btn-premium").First(b => b.TextContent == "Válido");
        validoBtn.Click();

        // Assert - SubmitVerificationDecisionAsync was called with correct parameters
        Assert.True(fakeService.DecisionSubmitted);
        Assert.Equal(101, fakeService.LastSubmittedOrderId);
        Assert.Equal("Válido", fakeService.LastSubmittedDecision);
        Assert.Equal("Todo correcto", fakeService.LastSubmittedObservation);
    }

    private class FakeBackofficeService : IBackofficeService
    {
        public bool DecisionSubmitted { get; private set; }
        public long LastSubmittedOrderId { get; private set; }
        public string? LastSubmittedDecision { get; private set; }
        public string? LastSubmittedObservation { get; private set; }

        public Task<IEnumerable<SalesQueueItem>> GetPendingQueueAsync()
        {
            var list = new List<SalesQueueItem>
            {
                new(101, "Carlos Mendoza", "Alta", DateTime.Now.AddMinutes(-15), "Pendiente DNI"),
                new(102, "Ana Lucía Rojas", "Media", DateTime.Now.AddMinutes(-45), "Pendiente DNI"),
                new(103, "Jorge Pérez", "Baja", DateTime.Now.AddHours(-2), "Pendiente DNI")
            };
            return Task.FromResult<IEnumerable<SalesQueueItem>>(list);
        }

        public Task<DocumentVerificationData?> GetVerificationDataAsync(long idOrder)
        {
            var data = new DocumentVerificationData(
                idOrder,
                $"/api/documents/{idOrder}/download",
                "Carlos Mendoza",
                "74859612",
                "Carlos Mendoza OCR",
                "74859612-OCR"
            );
            return Task.FromResult<DocumentVerificationData?>(data);
        }

        public Task<IEnumerable<OpenIncidentItem>> GetOpenIncidentsAsync(long idOrder)
        {
            var list = new List<OpenIncidentItem>
            {
                new(501, "Error de validación en buró", "Abierta", DateTime.Now.AddDays(-1))
            };
            return Task.FromResult<IEnumerable<OpenIncidentItem>>(list);
        }

        public Task SubmitVerificationDecisionAsync(long idOrder, string decision, string? observation)
        {
            DecisionSubmitted = true;
            LastSubmittedOrderId = idOrder;
            LastSubmittedDecision = decision;
            LastSubmittedObservation = observation;
            return Task.CompletedTask;
        }

        public Task<IEnumerable<BackofficeIncidentDto>> GetIncidentsAsync(string? role = "BAC", string? status = "OPEN")
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<KbArticleSuggestionDto>> GetKbSuggestionsAsync(long idIncident)
        {
            throw new NotImplementedException();
        }

        public Task AddIncidentResponseAsync(long idIncident, string responseText, string responseType, long respondedBy)
        {
            throw new NotImplementedException();
        }

        public Task ResolveIncidentAsync(long idIncident, string notes, long resolvedBy)
        {
            throw new NotImplementedException();
        }
    }
}
