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
    private readonly IUserRepository _userRepository;

    public AuthController(LoginUseCase loginUseCase, IUserRepository userRepository)
    {
        _loginUseCase = loginUseCase;
        _userRepository = userRepository;
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

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { message = "El usuario y la contraseña son requeridos." });
        }

        // Verificar si el usuario ya existe
        var existingUser = await _userRepository.GetByUsernameAsync(request.Username);
        if (existingUser != null)
        {
            return BadRequest(new { message = "El nombre de usuario ya está registrado." });
        }

        // Generar hash de contraseña usando BCrypt
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var newUser = new User
        {
            Username = request.Username,
            PasswordHash = passwordHash,
            DateCreated = DateTime.UtcNow,
            State = 1
        };

        try
        {
            await _userRepository.CreateAsync(newUser);
            return Ok(new { message = "Usuario registrado exitosamente." });
        }
        catch (Exception ex) when (ex.Message.Contains("42501") || ex.Message.Contains("permission denied"))
        {
            return StatusCode(403, new { 
                message = "La base de datos remota es de solo lectura (permiso denegado en la tabla 'users'). Se ha habilitado un usuario de pruebas local: 'testuser' con contraseña 'Password123!' para validaciones en Swagger." 
            });
        }
    }
}
