using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShippingRequestSystem.Models;

[Table("prcPurchaseShippingRequest")]
public class ShippingRequest
{
    [Key]
    public int Id { get; set; }
    [MaxLength(20)]
    public string? Folio { get; set; }

    [Required, MaxLength(200)]
    public string RequestorName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(200)]
    public string RequestorEmail { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Department { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string BusinessUnit { get; set; } = string.Empty;

    public DateTime RequestDate { get; set; } = DateTime.Now;

    // Alcance primero: Nacional -> NationalCategory (Compras/Adquisición); Internacional -> Direction (Export/Import)
    public ShipmentScope Scope { get; set; }
    public ShipmentDirection Direction { get; set; }
    public NationalShipmentCategory? NationalCategory { get; set; }

    // Buyer Info
    [MaxLength(300)]
    public string? BuyerBusinessName { get; set; }
    [MaxLength(500)]
    public string? BuyerAddress { get; set; }
    [MaxLength(100)]
    public string? BuyerTaxId { get; set; }

    // Recipient Info
    [MaxLength(300)]
    public string? RecipientBusinessName { get; set; }
    [MaxLength(500)]
    public string? RecipientAddress { get; set; }
    [MaxLength(100)]
    public string? RecipientTaxId { get; set; }
    [MaxLength(200)]
    public string? RecipientAttentionTo { get; set; }
    [MaxLength(50)]
    public string? RecipientPhone { get; set; }
    [MaxLength(200)]
    public string? RecipientEmail { get; set; }

    // Shipping Details
    public ShippingWay ShippingWay { get; set; }
    public ServiceType ServiceType { get; set; }
    public DateTime? DeliveryDate { get; set; }

    [MaxLength(1000)]
    public string? ShipmentPurpose { get; set; }

    // Invoice Info
    public InvoiceType InvoiceType { get; set; }

    // El solicitante marca el checklist de tramos de flete aplicables;
    // Trade Staff determina el Incoterm en su revisión (ver IncotermStaff-like flujo).
    public bool RequiresLocalOriginFreight { get; set; }
    public bool RequiresLocalDestinationFreight { get; set; }
    public bool RequiresInternationalFreight { get; set; }
    [MaxLength(50)]
    public string? Incoterm { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal IgiPercentage { get; set; }  // Se mantiene pero se llena por staff
    public bool HasCOO { get; set; }  // lo determina Trade Staff en su revisión
    [MaxLength(300)]
    public string? CooDocumentPath { get; set; }  // documento subido por Trade Staff
    [MaxLength(100)]
    public string? InvoiceNumber { get; set; }  // solo aplica cuando Scope == Internacional y Direction == Import
    public OperationType OperationType { get; set; }

    public bool IsStackable { get; set; }

    // Payment
    public PaymentResponsibility PaymentResponsibility { get; set; }
    [MaxLength(100)]
    public string? ParcelAccount { get; set; }
    public ParcelCarrier? Carrier { get; set; }  // solo aplica cuando PaymentResponsibility == Receiver

    public bool HasInsurance { get; set; }

    // Observations
    [MaxLength(2000)]
    public string? RequestorObservations { get; set; }
    [MaxLength(100)]
    public string? MaterialBajaFolio { get; set; }
    [MaxLength(2000)]
    public string? LogisticsStaffObservations { get; set; }

    // Evidencias fotográficas obligatorias (un set por solicitud)
    [MaxLength(300)]
    public string? EvidenceNetWeightPhotoPath { get; set; }
    [MaxLength(300)]
    public string? EvidenceGrossWeightPhotoPath { get; set; }
    [MaxLength(300)]
    public string? EvidenceDimensionsPhotoPath { get; set; }
    [MaxLength(300)]
    public string? EvidenceFinalPackagingPhotoPath { get; set; }

    // Borrador: paso del wizard en el que se quedó el solicitante
    public int DraftStep { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal ApproximateCost { get; set; } // Lo llena el sistema al seleccionar cotización

    public bool IsPremiumFreight => ApproximateCost >= 5000;

    public RequestStatus Status { get; set; } = RequestStatus.Draft;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Manager selected by requestor
    [MaxLength(200)]
    public string? UserManagerName { get; set; }
    [MaxLength(200)]
    public string? UserManagerEmail { get; set; }

    // Llenado por Trade Staff
    [Column(TypeName = "decimal(18,4)")]
    public decimal? IgiPercentageStaff { get; set; }

    // Cotización seleccionada por el usuario
    public int? SelectedQuoteId { get; set; }

    // Navigation
    public ICollection<ShippingQuote> Quotes { get; set; } = new List<ShippingQuote>();

    // Navigation
    public ICollection<ShippingPackage> Packages { get; set; } = new List<ShippingPackage>();
    public PremiumFreightInfo? PremiumFreightInfo { get; set; }
    public ICollection<ApprovalStep> ApprovalSteps { get; set; } = new List<ApprovalStep>();
}