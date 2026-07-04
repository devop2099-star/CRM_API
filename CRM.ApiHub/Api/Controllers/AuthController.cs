using System;
using System.Security.Claims;
using System.Threading.Tasks;
using CRM.ApiHub.Application.DTOs;
using CRM.ApiHub.Application.UseCases.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.ApiHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly LoginUseCase _loginUseCase;
    private readonly MeUseCase _meUseCase;
    private readonly RefreshTokenUseCase _refreshTokenUseCase;
    private readonly CRM.ApiHub.Application.Interfaces.IJwtTokenGenerator _tokenGenerator;

    public AuthController(
        LoginUseCase loginUseCase,
        MeUseCase meUseCase,
        RefreshTokenUseCase refreshTokenUseCase,
        CRM.ApiHub.Application.Interfaces.IJwtTokenGenerator tokenGenerator)
    {
        _loginUseCase = loginUseCase;
        _meUseCase = meUseCase;
        _refreshTokenUseCase = refreshTokenUseCase;
        _tokenGenerator = tokenGenerator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _loginUseCase.ExecuteAsync(request);
        if (response == null)
        {
            return Unauthorized(new { message = "Nombre de usuario o contraseña incorrectos." });
        }
        return Ok(response);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        // El claim de ID de usuario puede venir mapeado como NameIdentifier o directamente como "sub"
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out long userId))
        {
            return Unauthorized(new { message = "Usuario no autorizado o token inválido." });
        }

        var userDetail = await _meUseCase.ExecuteAsync(userId);
        if (userDetail == null)
        {
            return NotFound(new { message = "Usuario no encontrado." });
        }

        return Ok(new
        {
            nombre = userDetail.Username,
            rol = userDetail.RoleName,
            campanaAsignada = userDetail.CampaignName
        });
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var response = await _refreshTokenUseCase.ExecuteAsync(request);
        if (response == null)
        {
            return Unauthorized(new { message = "Token de refresco inválido o expirado." });
        }
        return Ok(response);
    }
}
