using CRM.ApiHub.Domain.Entities;
using CRM.ApiHub.Domain.DTOs;

namespace CRM.ApiHub.Domain.Repositories
{
    public interface IApprovalRepository
    {
        Task<long> RegisterApprovalAsync(ApprovalDto approval);
        // Podrías necesitar un método para actualizar el estado de la preventa/orden
    }
}