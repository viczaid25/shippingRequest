using ShippingRequestSystem.Models;

namespace ShippingRequestSystem.ViewModels;

public class PackageViewModel
{
    public int TempId { get; set; } = Random.Shared.Next(1, 999999);
    public PackageType PackageType { get; set; } = PackageType.Box;
    public string Label { get; set; } = string.Empty;

    // Peso bruto y dimensiones de este contenedor completo (caja o pallet armado).
    // Se captura una sola vez por caja/pallet, no por item, para no repetirlo por partida.
    public decimal GrossWeight { get; set; }
    public decimal Length { get; set; }
    public decimal Width { get; set; }
    public decimal Height { get; set; }

    // Solo relevante si es Pallet
    public List<PackageViewModel> ChildBoxes { get; set; } = new();

    // Items directos (si es Box sin pallet padre, o box dentro de pallet)
    public List<ItemViewModel> Items { get; set; } = new();

    public bool IsExpanded { get; set; } = true;
}