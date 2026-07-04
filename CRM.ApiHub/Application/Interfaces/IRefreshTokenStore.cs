using System;

namespace CRM.ApiHub.Application.Interfaces;

public interface IRefreshTokenStore
{
    void SaveToken(string token, long userId, DateTime expiry);
    bool TryGetUserId(string token, out long userId);
    void RevokeToken(string token);
}
