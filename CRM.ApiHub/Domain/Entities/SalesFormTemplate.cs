using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.ApiHub.Domain.Entities;

[Table("sales_form_template", Schema = "sales_service")]
public class SalesFormTemplate
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id_form")]
    public long IdForm { get; set; }

    [Column("id_cmpg")]
    public long IdCmpg { get; set; }

    [Column("id_status")]
    public long IdStatus { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("is_active")]
    public bool IsActive { get; set; } = true;
}
