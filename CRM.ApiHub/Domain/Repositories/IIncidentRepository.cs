using CRM.ApiHub.Domain.Entities;

namespace CRM.ApiHub.Domain.Repositories;

public interface IIncidentRepository
{
    Task<IEnumerable<IncidentCatalog>> GetCatalogAsync(long idCmpg, long idStatus);
    Task<IEnumerable<OrderIncident>> GetByOrderAsync(long idOrder);
    Task<long> CreateAsync(OrderIncident incident);
    Task CreateResponseAsync(long idOrderIncident, string responseText, string responseType, long respondedBy);
    Task ResolveAsync(long idOrderIncident, string notes, long resolvedBy);
    
    Task<IEnumerable<KbArticleSuggestion>> GetKbSuggestionsAsync(long idIncident);
}