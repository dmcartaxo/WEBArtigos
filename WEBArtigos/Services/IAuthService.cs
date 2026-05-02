using WEBArtigos.DTOs;

namespace WEBArtigos.Services;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto);
}
