using Dapper;
using System.Threading.Tasks;
using CRM.ApiHub.Domain.Entities; 
using CRM.ApiHub.Domain.Repositories;
using CRM.ApiHub.Domain.DTOs;

namespace CRM.ApiHub.Infrastructure.Persistence
{
    public class ApprovalRepository : IApprovalRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public ApprovalRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<long> RegisterApprovalAsync(ApprovalDto approval)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync();

            // 1. Obtener el id_user de la orden para usarlo como requested_by
            const string getOrderUserSql = "SELECT id_user FROM sales_service.sales_order WHERE id_order = @IdOrder;";
            var requestedBy = await connection.ExecuteScalarAsync<long?>(getOrderUserSql, new { IdOrder = approval.IdOrder });

            // Si no se encuentra la orden en sales_order, usamos authorized_by como fallback
            var finalRequestedBy = requestedBy ?? approval.AuthorizedBy;

            string statusValue = approval.IsApproved ? "APPROVED" : "REJECTED";
            string typeValue = "MANUAL";

            var sql = @"INSERT INTO sales_service.sales_order_approval 
                        (id_order, approved_by, approval_reason, rejection_reason, status, register, approval_type, requested_by, resolved_at) 
                        VALUES (@IdOrder, @AuthorizedBy, @ApprovalReason, @RejectionReason, @Status, NOW(), @Type, @RequestedBy, NOW()) 
                        RETURNING id_approval;";
            
            return await connection.ExecuteScalarAsync<long>(sql, new 
            {
                approval.IdOrder,
                approval.AuthorizedBy,
                ApprovalReason = approval.IsApproved ? approval.Comments : null,
                RejectionReason = !approval.IsApproved ? approval.Comments : null,
                Status = statusValue,
                Type = typeValue,
                RequestedBy = finalRequestedBy
            });
        }
    }
}