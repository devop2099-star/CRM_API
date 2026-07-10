using System.Collections.Generic;

namespace CRM.ApiHub.Application.DTOs;

public class FormFieldDto
{
    public long IdFld { get; set; }
    public string Label { get; set; } = string.Empty;
    public string FieldKey { get; set; } = string.Empty;
    public string FieldType { get; set; } = string.Empty;
    public bool IsRequired { get; set; } = true;
    public string? ValidationRegex { get; set; }
    public string? Options { get; set; }
    public int OrderIndex { get; set; }
    public string? ValidationType { get; set; }
    public string? Placeholder { get; set; }
    public string? HelpText { get; set; }
    public string? GroupName { get; set; }
    public long? DependsOnField { get; set; }
    public string? DependsOnValue { get; set; }
}

public class FormSchemaDto
{
    public long IdForm { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<FormFieldDto> Fields { get; set; } = new();
}
