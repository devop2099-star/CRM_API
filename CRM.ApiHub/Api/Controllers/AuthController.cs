using System;
using System.Threading.Tasks;
using CRM.ApiHub.Application.DTOs;
using CRM.ApiHub.Application.UseCases.Auth;
using CRM.ApiHub.Domain.Entities;
using CRM.ApiHub.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CRM.ApiHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly LoginUseCase _loginUseCase;

    public AuthController(LoginUseCase loginUseCase)
    {
        _loginUseCase = loginUseCase;
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
}
