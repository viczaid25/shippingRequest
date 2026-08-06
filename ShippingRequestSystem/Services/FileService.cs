namespace ShippingRequestSystem.Services;

public class FileService
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<FileService> _logger;

    // Tamaño máximo: 10 MB
    private const long MaxFileSizeBytes = 10 * 1024 * 1024;

    public FileService(IWebHostEnvironment env, ILogger<FileService> logger)
    {
        _env = env;
        _logger = logger;
    }

    /// <summary>
    /// Guarda un PDF en wwwroot/uploads/quotes/ y retorna la ruta relativa.
    /// </summary>
    public async Task<(string filePath, string fileName)?> SaveQuotePdfAsync(
        Stream fileStream, string originalFileName, long fileSize)
    {
        if (fileSize > MaxFileSizeBytes)
            throw new Exception($"El archivo excede el tamaño máximo de 10 MB.");

        var ext = Path.GetExtension(originalFileName).ToLowerInvariant();
        if (ext != ".pdf")
            throw new Exception("Solo se permiten archivos PDF.");

        var uploadDir = Path.Combine(_env.WebRootPath, "uploads", "quotes");
        Directory.CreateDirectory(uploadDir);

        // Nombre único para evitar colisiones
        var uniqueName = $"{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(uploadDir, uniqueName);

        await using var fs = new FileStream(fullPath, FileMode.Create);
        await fileStream.CopyToAsync(fs);

        // Ruta relativa para guardar en DB y generar URL
        var relativePath = $"/uploads/quotes/{uniqueName}";

        _logger.LogInformation("PDF guardado: {Path}", relativePath);
        return (relativePath, originalFileName);
    }

    /// <summary>
    /// Guarda el documento de Certificado de Origen (PDF) en wwwroot/uploads/coo/ y retorna la ruta relativa.
    /// </summary>
    public async Task<(string filePath, string fileName)> SaveCooDocumentAsync(
        Stream fileStream, string originalFileName, long fileSize)
    {
        if (fileSize > MaxFileSizeBytes)
            throw new Exception("El archivo excede el tamaño máximo de 10 MB.");

        var ext = Path.GetExtension(originalFileName).ToLowerInvariant();
        if (ext != ".pdf")
            throw new Exception("Solo se permiten archivos PDF.");

        var uploadDir = Path.Combine(_env.WebRootPath, "uploads", "coo");
        Directory.CreateDirectory(uploadDir);

        var uniqueName = $"{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(uploadDir, uniqueName);

        await using var fs = new FileStream(fullPath, FileMode.Create);
        await fileStream.CopyToAsync(fs);

        var relativePath = $"/uploads/coo/{uniqueName}";

        _logger.LogInformation("Documento COO guardado: {Path}", relativePath);
        return (relativePath, originalFileName);
    }

    /// <summary>
    /// Guarda una foto de evidencia (JPG/PNG) en wwwroot/uploads/evidence/ y retorna la ruta relativa.
    /// </summary>
    public async Task<(string filePath, string fileName)> SaveEvidenceImageAsync(
        Stream fileStream, string originalFileName, long fileSize)
    {
        const long maxEvidenceSizeBytes = 8 * 1024 * 1024;
        if (fileSize > maxEvidenceSizeBytes)
            throw new Exception("La imagen excede el tamaño máximo de 8 MB.");

        var ext = Path.GetExtension(originalFileName).ToLowerInvariant();
        if (ext != ".jpg" && ext != ".jpeg" && ext != ".png")
            throw new Exception("Solo se permiten imágenes JPG o PNG.");

        var uploadDir = Path.Combine(_env.WebRootPath, "uploads", "evidence");
        Directory.CreateDirectory(uploadDir);

        var uniqueName = $"{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(uploadDir, uniqueName);

        await using var fs = new FileStream(fullPath, FileMode.Create);
        await fileStream.CopyToAsync(fs);

        var relativePath = $"/uploads/evidence/{uniqueName}";

        _logger.LogInformation("Evidencia guardada: {Path}", relativePath);
        return (relativePath, originalFileName);
    }

    public void DeleteFile(string? relativePath)
    {
        if (string.IsNullOrEmpty(relativePath)) return;
        try
        {
            var fullPath = Path.Combine(_env.WebRootPath,
                relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("No se pudo eliminar archivo {Path}: {Msg}",
                relativePath, ex.Message);
        }
    }
}