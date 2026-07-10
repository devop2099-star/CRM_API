using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CRM.ApiHub.Application.DTOs;
using CRM.ApiHub.Domain.Repositories;

namespace CRM.ApiHub.Application.UseCases.SalesOrders;

public class GetFormSchemaUseCase
{
    private readonly IFormSchemaRepository _formSchemaRepository;

    public GetFormSchemaUseCase(IFormSchemaRepository formSchemaRepository)
    {
        _formSchemaRepository = formSchemaRepository;
    }

    public async Task<FormSchemaDto?> ExecuteAsync(long idCmpg, long idStatus, CancellationToken ct = default)
    {
        var template = await _formSchemaRepository.GetTemplateAsync(idCmpg, idStatus, ct);
        if (template == null)
            return null;

        var fields = await _formSchemaRepository.GetFieldsByTemplateIdAsync(template.IdForm, ct);

        var dto = new FormSchemaDto
        {
            IdForm = template.IdForm,
            Name = template.Name,
            Fields = fields.Select(f => new FormFieldDto
            {
                IdFld = f.IdFld,
                Label = f.Label,
                FieldKey = f.FieldKey,
                FieldType = f.FieldType,
                IsRequired = f.IsRequired,
                ValidationRegex = f.ValidationRegex,
                Options = f.Options,
                OrderIndex = f.OrderIndex,
                ValidationType = f.ValidationType,
                Placeholder = f.Placeholder,
                HelpText = f.HelpText,
                GroupName = f.GroupName,
                DependsOnField = f.DependsOnField,
                DependsOnValue = f.DependsOnValue
            }).ToList()
        };

        return dto;
    }
}
