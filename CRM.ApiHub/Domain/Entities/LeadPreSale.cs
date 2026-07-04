using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.ApiHub.Domain.Entities;

[Table("lead_pre_sale")]
public class LeadPreSale
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Column("lead_id")]
    public int LeadId { get; set; }

    [Column("assigned_user_id")]
    public int AssignedUserId { get; set; }

    [Column("notes")]
    public string? Notes { get; set; }

    [Column("scheduled_contact_date")]
    public DateTime? ScheduledContactDate { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}