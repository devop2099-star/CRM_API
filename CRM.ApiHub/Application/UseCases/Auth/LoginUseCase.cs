using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using CRM.ApiHub.Application.DTOs;
using CRM.ApiHub.Application.Interfaces;
using CRM.ApiHub.Domain.Repositories;

namespace CRM.ApiHub.Application.UseCases.Auth;

public class LoginUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly IRefreshTokenStore _refreshTokenStore;

    public LoginUseCase(
        IUserRepository userRepository, 
        IJwtTokenGenerator tokenGenerator,
        IRefreshTokenStore refreshTokenStore)
    {
        _userRepository = userRepository;
        _tokenGenerator = tokenGenerator;
        _refreshTokenStore = refreshTokenStore;
    }

    public async Task<LoginResponse?> ExecuteAsync(LoginRequest request, CancellationToken ct = default)
    {
        // 1. Obtener el usuario por username
        var user = await _userRepository.GetByUsernameAsync(request.Username, ct);
        if (user == null)
        {
            return null;
        }

        // 2. Verificar la contraseña usando BCrypt
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            return null;
        }

        // 3. Obtener el rol del usuario
        var userDetail = await _userRepository.GetUserDetailByIdAsync(user.IdUser, ct);
        var role = userDetail?.RoleName;

        // 4. Generar token JWT válido
        var token = _tokenGenerator.GenerateToken(user, role);

        // 5. Generar Refresh Token
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var expiry = DateTime.UtcNow.AddDays(7);
        _refreshTokenStore.SaveToken(refreshToken, user.IdUser, expiry);

        return new LoginResponse(token, refreshToken, user.Username, role);
    }
}
