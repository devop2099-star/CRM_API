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

    public LoginUseCase(IUserRepository userRepository, IJwtTokenGenerator tokenGenerator)
    {
        _userRepository = userRepository;
        _tokenGenerator = tokenGenerator;
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

        // 3. Generar token JWT válido
        var token = _tokenGenerator.GenerateToken(user);

        return new LoginResponse(token, user.Username);
    }
}
