using System.Text;
using System.Text.Json;

namespace WEBArtigos.Services;

public class AiService(
    IHttpClientFactory httpClientFactory,
    IConfiguration config,
    ILogger<AiService> logger) : IAiService
{
    private const string ApiUrl = "https://api.anthropic.com/v1/messages";
    private const int MaxInputChars = 50_000;

    public async Task<string> SummarizeAsync(string text)
    {
        var model = config["Anthropic:Model"] ?? "claude-sonnet-4-6";

        // Trunca para não exceder o contexto (PDFs muito longos)
        var truncated = text.Length > MaxInputChars ? text[..MaxInputChars] : text;

        logger.LogInformation("Chamando IA para resumo — modelo={Model}, chars={Chars}", model, truncated.Length);

        var requestBody = new
        {
            model,
            max_tokens = 1024,
            messages = new[]
            {
                new
                {
                    role = "user",
                    content = $"""
                        Leia o seguinte texto extraído de um artigo de jornal e escreva um resumo conciso em português.
                        O resumo deve:
                        - Ter entre 5 e 10 linhas
                        - Destacar os pontos principais
                        - Ser objetivo e informativo
                        - Não incluir introduções como "Este artigo trata de..."

                        Texto do artigo:
                        {truncated}
                        """
                }
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response;
        try
        {
            var client = httpClientFactory.CreateClient("Anthropic");
            response = await client.PostAsync(ApiUrl, httpContent);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Falha na comunicação com a API de IA");
            throw new InvalidOperationException("Não foi possível conectar ao serviço de IA.", ex);
        }

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            logger.LogError("API de IA retornou erro {Status}: {Body}", response.StatusCode, error);
            throw new InvalidOperationException($"Serviço de IA retornou erro: {response.StatusCode}.");
        }

        var responseJson = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(responseJson);
        var summary = doc.RootElement
            .GetProperty("content")[0]
            .GetProperty("text")
            .GetString();

        if (string.IsNullOrWhiteSpace(summary))
            throw new InvalidOperationException("A IA retornou um resumo vazio.");

        logger.LogInformation("Resumo gerado com sucesso: {Chars} caracteres", summary.Length);
        return summary.Trim();
    }
}
