using WEBArtigos.DTOs;

namespace WEBArtigos.Services;

public interface IArticleService
{
    Task<IEnumerable<ArticleResponseDto>> GetAllAsync();
    Task<ArticleResponseDto?> GetByIdAsync(int id);
    Task<IEnumerable<ArticleResponseDto>> SearchAsync(string query);
    Task<ArticleResponseDto> CreateAsync(ArticleCreateDto dto);
    Task<ArticleResponseDto?> UpdateAsync(int id, ArticleUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}
