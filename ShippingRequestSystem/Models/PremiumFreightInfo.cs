using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShippingRequestSystem.Models;

[Table("prcPurchasePremiumFreightInfo")]
public class PremiumFreightInfo
{
    [Key]
    public int Id { get; set; }
    [MaxLength(20)]
    public string? Folio { get; set; }

    public int ShippingRequestId { get; set; }
    public ShippingRequest ShippingRequest { get; set; } = null!;

    public PremiumFreightDirection Direction { get; set; }
    public PremiumFreightScope Scope { get; set; }
    public PremiumFreightTransport Transport { get; set; }
    public PremiumFreightPayment Payment { get; set; }

    // Pick Up
    [MaxLength(300)]
    public string? PickUpName { get; set; }
    [MaxLength(500)]
    public string? PickUpAddress { get; set; }
    [MaxLength(200)]
    public string? PickUpContact { get; set; }

    // Delivery
    [MaxLength(300)]
    public string? DeliveryName { get; set; }
    [MaxLength(500)]
    public string? DeliveryAddress { get; set; }
    [MaxLength(200)]
    public string? DeliveryContact { get; set; }

    [MaxLength(2000)]
    public string? CauseOfShipment { get; set; }

    [MaxLength(2000)]
    public string? Countermeasures { get; set; }

    [MaxLength(300)]
    public string? Forwarder { get; set; }
}