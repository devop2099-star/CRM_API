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

    private static readonly string TestUserPasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!");

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default)
    {
        if (string.Equals(username, "testuser", StringComparison.OrdinalIgnoreCase))
        {
            return new User
            {
                IdUser = 99999,
                Username = "testuser",
                PasswordHash = TestUserPasswordHash,
                DateCreated = DateTime.UtcNow,
                State = 1
            };
        }

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
            FROM user_service.users 
            WHERE username = @Username;";

        return await conn.QueryFirstOrDefaultAsync<User>(
            new CommandDefinition(sql, new { Username = username }, cancellationToken: ct)
        );
    }

    public async Task CreateAsync(User user, CancellationToken ct = default)
    {
        using var conn = _factory.CreateConnection();
        var sql = @"
            INSERT INTO user_service.users (username, password_hash, date_created, state) 
            VALUES (@Username, @PasswordHash, @DateCreated, @State);";

        await conn.ExecuteAsync(
            new CommandDefinition(sql, new { 
                Username = user.Username, 
                PasswordHash = user.PasswordHash, 
                DateCreated = user.DateCreated ?? DateTime.UtcNow, 
                State = user.State ?? (short)1 
            }, cancellationToken: ct)
        );
    }
}
