using System.ComponentModel.DataAnnotations;

namespace WEBArtigos.DTOs;

public class ArticleUpdateDto
{
    [Required(ErrorMessage = "Título é obrigatório.")]
    [StringLength(300, MinimumLength = 3, ErrorMessage = "Título deve ter entre 3 e 300 caracteres.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Conteúdo é obrigatório.")]
    [MinLength(10, ErrorMessage = "Conteúdo deve ter no mínimo 10 caracteres.")]
    public string Content { get; set; } = string.Empty;

    [Required(ErrorMessage = "Autor é obrigatório.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Autor deve ter entre 2 e 150 caracteres.")]
    public string Author { get; set; } = string.Empty;
}
