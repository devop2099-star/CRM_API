using System.Threading;
using System.Threading.Tasks;
using CRM.ApiHub.Domain.Entities;

namespace CRM.ApiHub.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default);
    Task CreateAsync(User user, CancellationToken ct = default);
}
