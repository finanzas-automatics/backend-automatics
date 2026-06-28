using Microsoft.AspNetCore.Mvc;
using AutomaticsApi.Models.DTOs;
using AutomaticsApi.Services;

namespace AutomaticsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    /// <summary>Login — devuelve token JWT</summary>
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        if (result is null)
            return Unauthorized(new ApiResponse<LoginResponse>(false, "Credenciales incorrectas", null));

        return Ok(new ApiResponse<LoginResponse>(true, "Login exitoso", result));
    }

    /// <summary>Registrar nuevo asesor</summary>
    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<string>>> Register([FromBody] RegisterRequest request)
    {
        var ok = await _authService.RegisterAsync(request);
        if (!ok)
            return Conflict(new ApiResponse<string>(false, "El correo ya está registrado", null));

        return Ok(new ApiResponse<string>(true, "Usuario registrado correctamente", null));
    }
}
