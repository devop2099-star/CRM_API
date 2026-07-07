namespace CRM.WebFrontend.Models;

public record SalesQueueItem(
    long IdOrder, 
    string CustomerName, 
    string Priority, 
    DateTime SubmittedAt, 
    string Status
);

public record DocumentVerificationData(
    long IdOrder, 
    string DocumentImageUrl, 
    string FormFullName, 
    string FormDocumentNumber,
    string ScannedFullName, 
    string ScannedDocumentNumber
);

public record OpenIncidentItem(
    long IdIncident, 
    string Description, 
    string Status, 
    DateTime CreatedAt
);