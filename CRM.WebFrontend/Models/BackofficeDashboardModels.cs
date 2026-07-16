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

public record BackofficeIncidentDto(
    long IdOrderIncident,
    long IdOrder,
    long IdIncident,
    string? CustomName,
    string? CustomDescription,
    string? IncidentStatus,
    string? AssignedToRole,
    DateTime? DueAt,
    DateTime Register,
    short? Priority = null
);

public record KbArticleSuggestionDto(
    long IdArticle,
    string Title,
    string Summary,
    string Slug
);