using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using AutomaticsApi.Data;
using AutomaticsApi.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using AutomaticsApi.Models.Entities;

namespace AutomaticsApi.Services;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<bool> RegisterAsync(RegisterRequest request);
}

public class AuthService : IAuthService
{
    private readonly AutomaticsDbContext _db;
    private readonly IConfiguration _config;

    public AuthService(AutomaticsDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user is null) return null;
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash)) return null;

        var token = GenerateToken(user);
        var expires = DateTime.UtcNow.AddHours(8);

        return new LoginResponse(token, user.Name, user.Email, user.Role, user.Dni, expires);
    }

    public async Task<bool> RegisterAsync(RegisterRequest request)
    {
        if (await _db.Users.AnyAsync(u => u.Email == request.Email))
            return false;

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role,
            Dni = request.Dni
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return true;
    }

    private string GenerateToken(User user)
    {
        var jwtKey = _config["Jwt:Key"]!;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
