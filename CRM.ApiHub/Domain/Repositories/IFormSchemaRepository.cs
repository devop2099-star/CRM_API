using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CRM.ApiHub.Domain.Entities;

namespace CRM.ApiHub.Domain.Repositories;

public interface IFormSchemaRepository
{
    Task<SalesFormTemplate?> GetTemplateAsync(long idCmpg, long idStatus, CancellationToken ct = default);
    Task<IEnumerable<SalesFormField>> GetFieldsByTemplateIdAsync(long idForm, CancellationToken ct = default);
}
