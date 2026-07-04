using System;
using System.Collections.Concurrent;
using CRM.ApiHub.Application.Interfaces;

namespace CRM.ApiHub.Infrastructure.Authentication;

public class InMemoryRefreshTokenStore : IRefreshTokenStore
{
    private readonly ConcurrentDictionary<string, (long UserId, DateTime Expiry)> _tokens = new();

    public void SaveToken(string token, long userId, DateTime expiry)
    {
        _tokens[token] = (userId, expiry);
    }

    public bool TryGetUserId(string token, out long userId)
    {
        userId = 0;
        if (_tokens.TryGetValue(token, out var val))
        {
            if (val.Expiry > DateTime.UtcNow)
            {
                userId = val.UserId;
                return true;
            }
            _tokens.TryRemove(token, out _);
        }
        return false;
    }

    public void RevokeToken(string token)
    {
        _tokens.TryRemove(token, out _);
    }
}
