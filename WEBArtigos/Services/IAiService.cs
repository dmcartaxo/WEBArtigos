namespace WEBArtigos.Services;

public interface IAiService
{
    Task<string> SummarizeAsync(string text);
}
