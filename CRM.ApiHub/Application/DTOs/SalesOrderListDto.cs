using System;

namespace CRM.ApiHub.Application.DTOs;

public class SalesOrderListDto
{
    public long IdOrder { get; set; }
    public long IdLead { get; set; }
    public long IdCmpg { get; set; }
    public long IdUser { get; set; }
    public long? OwnerUserId { get; set; }
    public long? CustodyUserId { get; set; }
    public long? IdStatus { get; set; }
    public long? IdSubstatus { get; set; }
    
    // Joined text fields for UI
    public string ClientName { get; set; } = string.Empty;
    public string CampaignName { get; set; } = string.Empty;
    public string StatusName { get; set; } = string.Empty;
    public string? StatusColor { get; set; }
    public string? SubstatusName { get; set; }
    
    public string? CurrencyCode { get; set; }
    public string? CommissionCurrency { get; set; }
    public DateTime SalesDate { get; set; }
    public int TotalProducts { get; set; }
    public decimal? TotalValue { get; set; }
    public bool IsAlternate { get; set; }
    public DateTime Register { get; set; }
    
    public int DaysElapsed => (DateTime.UtcNow - Register).Days;
}
