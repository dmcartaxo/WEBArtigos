using Microsoft.AspNetCore.Mvc;
using WEBArtigos.DTOs;
using WEBArtigos.Services;

namespace WEBArtigos.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ArticlesController(IArticleService service) : ControllerBase
{
    /// <summary>Lista todos os artigos.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ArticleResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var articles = await service.GetAllAsync();
        return Ok(articles);
    }

    /// <summary>Busca artigos por texto no título ou conteúdo.</summary>
    /// <param name="query">Texto a pesquisar</param>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<ArticleResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Search([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest(new { message = "O parâmetro 'query' é obrigatório." });

        var results = await service.SearchAsync(query);
        return Ok(results);
    }

    /// <summary>Obtém um artigo pelo ID.</summary>
    /// <param name="id">ID do artigo</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ArticleResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var article = await service.GetByIdAsync(id);
        if (article is null)
            return NotFound(new { message = $"Artigo com ID {id} não encontrado." });

        return Ok(article);
    }

    /// <summary>Cria um novo artigo.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ArticleResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] ArticleCreateDto dto)
    {
        var created = await service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Atualiza um artigo existente.</summary>
    /// <param name="id">ID do artigo</param>
    /// <param name="dto">Dados para atualização</param>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ArticleResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] ArticleUpdateDto dto)
    {
        var updated = await service.UpdateAsync(id, dto);
        if (updated is null)
            return NotFound(new { message = $"Artigo com ID {id} não encontrado." });

        return Ok(updated);
    }

    /// <summary>Remove um artigo.</summary>
    /// <param name="id">ID do artigo</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await service.DeleteAsync(id);
        if (!deleted)
            return NotFound(new { message = $"Artigo com ID {id} não encontrado." });

        return NoContent();
    }
}
