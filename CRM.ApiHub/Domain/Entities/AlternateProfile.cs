using System;

namespace CRM.ApiHub.Domain.Entities
{
    public class AlternateProfile
    {
        public long IdAlternate { get; set; }
        public long IdOrder { get; set; }
        public string AlternateType { get; set; }
        public string AlternateData { get; set; } // Representando el jsonb como string
        public string OriginalData { get; set; }  // Representando el jsonb como string
        public string Reason { get; set; }
        public long CreatedBy { get; set; }
        public long? AuthorizedBy { get; set; } // Nullable porque es YES en la BD
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}