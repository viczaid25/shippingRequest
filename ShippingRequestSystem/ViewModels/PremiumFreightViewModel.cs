using ShippingRequestSystem.Models;

namespace ShippingRequestSystem.ViewModels;

public class PremiumFreightViewModel
{
    public PremiumFreightDirection Direction { get; set; } = PremiumFreightDirection.Export;
    public PremiumFreightScope Scope { get; set; } = PremiumFreightScope.Nacional;
    public PremiumFreightTransport Transport { get; set; } = PremiumFreightTransport.AirFreight;
    public PremiumFreightPayment Payment { get; set; } = PremiumFreightPayment.PaidByMeax;

    public string? PickUpName { get; set; }
    public string? PickUpAddress { get; set; }
    public string? PickUpContact { get; set; }

    public string? DeliveryName { get; set; }
    public string? DeliveryAddress { get; set; }
    public string? DeliveryContact { get; set; }

    public string? CauseOfShipment { get; set; }
    public string? Countermeasures { get; set; }
    public string? Forwarder { get; set; }
}