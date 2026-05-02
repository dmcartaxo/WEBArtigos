namespace WEBArtigos.DTOs;

public class ArticleUploadDto
{
    public IFormFile File { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
}
