namespace CRM.ApiHub.Domain.DTOs
{
    public class AlternateProfileDto 
    {
    public long IdOrder { get; set; }
        public string AlternateType { get; set; }
        public string AlternateData { get; set; } 
        public string OriginalData { get; set; }
        public string Reason { get; set; }
        public long CreatedBy { get; set; }
    }
}