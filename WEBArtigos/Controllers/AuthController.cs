using Microsoft.AspNetCore.Mvc;
using WEBArtigos.Common;
using WEBArtigos.DTOs;
using WEBArtigos.Services;

namespace WEBArtigos.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>Autentica o usuário e retorna um token JWT.</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        var result = await authService.LoginAsync(dto);

        if (result is null)
            return Unauthorized(ApiResponse<object>.Fail("Email ou senha inválidos."));

        return Ok(ApiResponse<LoginResponseDto>.Ok(result));
    }
}
