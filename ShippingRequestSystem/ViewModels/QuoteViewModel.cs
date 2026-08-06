using Microsoft.AspNetCore.Components.Forms;

namespace ShippingRequestSystem.ViewModels;

public class QuoteViewModel
{
    public string ProviderName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Notes { get; set; }

    // Archivo cargado en memoria antes de guardar
    public IBrowserFile? File { get; set; }
    public string? FileName { get; set; }
    public bool HasFile => File is not null || !string.IsNullOrEmpty(SavedFilePath);

    // Si ya tiene un archivo guardado (edición)
    public string? SavedFilePath { get; set; }
    public string? SavedFileName { get; set; }

    // Error de validación del archivo
    public string? FileError { get; set; }
}