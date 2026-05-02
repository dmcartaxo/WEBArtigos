namespace WEBArtigos.Services;

public class FileService(IConfiguration config) : IFileService
{
    private static readonly string[] AllowedContentTypes =
        ["application/pdf", "application/x-pdf", "application/octet-stream"];

    public void ValidateFile(IFormFile file)
    {
        if (file is null || file.Length == 0)
            throw new ArgumentException("Nenhum arquivo enviado ou arquivo vazio.");

        var maxSizeMb = config.GetValue<int>("Upload:MaxFileSizeMb", 10);
        var maxBytes = maxSizeMb * 1024L * 1024L;

        if (file.Length > maxBytes)
            throw new ArgumentException($"O arquivo excede o tamanho máximo permitido de {maxSizeMb} MB.");

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (extension != ".pdf")
            throw new ArgumentException("Apenas arquivos com extensão .pdf são aceitos.");

        // Valida pelo Content-Type, mas não confia cegamente (pode ser forjado)
        // A validação real é feita pelo PdfService ao tentar abrir o arquivo
        if (!AllowedContentTypes.Contains(file.ContentType.ToLowerInvariant()))
            throw new ArgumentException("O Content-Type do arquivo deve ser application/pdf.");
    }
}
