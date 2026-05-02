namespace WEBArtigos.Services;

public interface IPdfService
{
    Task<string> ExtractTextAsync(IFormFile file);
}
