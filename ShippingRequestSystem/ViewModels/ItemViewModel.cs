namespace ShippingRequestSystem.ViewModels;

public class ItemViewModel
{
    public int TempId { get; set; } = Random.Shared.Next(1, 999999);
    public string PartNumber { get; set; } = string.Empty;
    public string? Class { get; set; }
    public string? Description { get; set; }
    public int Quantity { get; set; } = 1;
    public string? UOM { get; set; }
    public decimal NetWeight { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Amount => Quantity * UnitPrice;
}