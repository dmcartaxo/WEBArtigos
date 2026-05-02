using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WEBArtigos.DTOs;
using WEBArtigos.Entities;
using WEBArtigos.Repositories;

namespace WEBArtigos.Services;

public class AuthService(
    IUserRepository userRepository,
    IConfiguration config,
    ILogger<AuthService> logger) : IAuthService
{
    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto)
    {
        var user = await userRepository.GetByEmailAsync(dto.Email.Trim().ToLower());

        // Erro genérico: não revela se o email existe ou não
        if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            logger.LogWarning("Tentativa de login inválida para: {Email}", dto.Email);
            return null;
        }

        var expirationHours = config.GetValue<int>("Jwt:ExpirationHours", 8);
        var expiresAt = DateTime.UtcNow.AddHours(expirationHours);
        var token = GenerateToken(user, expiresAt);

        logger.LogInformation("Login bem-sucedido: {Email} | Role={Role}", user.Email, user.Role);

        return new LoginResponseDto
        {
            Token = token,
            ExpiresAt = expiresAt,
            Email = user.Email,
            Role = user.Role
        };
    }

    private string GenerateToken(User user, DateTime expiresAt)
    {
        var secret = config["Jwt:Secret"]
            ?? throw new InvalidOperationException("JWT Secret não configurado.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
