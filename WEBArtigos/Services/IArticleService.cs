using WEBArtigos.Common;
using WEBArtigos.DTOs;

namespace WEBArtigos.Services;

public interface IArticleService
{
    Task<PagedResult<ArticleResponseDto>> GetPagedAsync(ArticleQueryDto query);
    Task<ArticleResponseDto?> GetByIdAsync(int id);
    Task<ArticleResponseDto> CreateAsync(ArticleCreateDto dto);
    Task<ArticleResponseDto?> UpdateAsync(int id, ArticleUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}
