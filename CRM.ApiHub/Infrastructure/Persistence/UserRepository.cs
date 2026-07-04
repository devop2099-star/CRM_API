using System;
using System.Threading;
using System.Threading.Tasks;
using CRM.ApiHub.Domain.Entities;
using CRM.ApiHub.Domain.Repositories;
using Dapper;

namespace CRM.ApiHub.Infrastructure.Persistence;

public class UserRepository : IUserRepository
{
    private readonly IDbConnectionFactory _factory;

    public UserRepository(IDbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default)
    {
        using var conn = _factory.CreateConnection();
        var sql = @"
            SELECT 
                id_user as IdUser, 
                username, 
                password_hash as PasswordHash, 
                date_created as DateCreated, 
                is_logged_in as IsLoggedIn, 
                last_activity as LastActivity, 
                fingerprint, 
                state 
            FROM users 
            WHERE username = @Username;";

        return await conn.QueryFirstOrDefaultAsync<User>(
            new CommandDefinition(sql, new { Username = username }, cancellationToken: ct)
        );
    }
}
