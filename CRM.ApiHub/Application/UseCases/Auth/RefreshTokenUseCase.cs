using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using CRM.ApiHub.Application.DTOs;
using CRM.ApiHub.Application.Interfaces;
using CRM.ApiHub.Domain.Repositories;

namespace CRM.ApiHub.Application.UseCases.Auth;

public class RefreshTokenUseCase
{
    private readonly IRefreshTokenStore _refreshTokenStore;
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public RefreshTokenUseCase(
        IRefreshTokenStore refreshTokenStore,
        IUserRepository userRepository,
        IJwtTokenGenerator tokenGenerator)
    {
        _refreshTokenStore = refreshTokenStore;
        _userRepository = userRepository;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<LoginResponse?> ExecuteAsync(RefreshTokenRequest request, CancellationToken ct = default)
    {
        if (!_refreshTokenStore.TryGetUserId(request.RefreshToken, out long userId))
        {
            return null;
        }

        var user = await _userRepository.GetByIdAsync(userId, ct);
        if (user == null || user.State != 1)
        {
            return null;
        }

        // Rotación de Refresh Token (revocar el anterior)
        _refreshTokenStore.RevokeToken(request.RefreshToken);

        // Generar nuevos tokens
        var newAccessToken = _tokenGenerator.GenerateToken(user);
        var newRefreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var expiry = DateTime.UtcNow.AddDays(7);
        _refreshTokenStore.SaveToken(newRefreshToken, user.IdUser, expiry);

        return new LoginResponse(newAccessToken, newRefreshToken, user.Username);
    }
}
