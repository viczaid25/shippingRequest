using System.ComponentModel.DataAnnotations;
using ShippingRequestSystem.Models;

namespace ShippingRequestSystem.ViewModels;

public class ShippingRequestViewModel
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [MaxLength(200)]
    public string RequestorName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string RequestorEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "El departamento es requerido")]
    public string Department { get; set; } = string.Empty;

    [Required(ErrorMessage = "La unidad de negocio es requerida")]
    public string BusinessUnit { get; set; } = string.Empty;

    public DateTime RequestDate { get; set; } = DateTime.Now;

    // Alcance primero: Nacional -> Categoría (Compras/Adquisición); Internacional -> Direction (Export/Import)
    public ShipmentScope Scope { get; set; } = ShipmentScope.Nacional;
    public ShipmentDirection Direction { get; set; } = ShipmentDirection.Export;
    public NationalShipmentCategory? NationalCategory { get; set; }

    public string? BuyerBusinessName { get; set; }
    public string? BuyerAddress { get; set; }
    public string? BuyerTaxId { get; set; }

    [Required(ErrorMessage = "La razón social del destinatario es requerida")]
    public string RecipientBusinessName { get; set; } = string.Empty;

    [Required(ErrorMessage = "La dirección del destinatario es requerida")]
    public string RecipientAddress { get; set; } = string.Empty;

    public string? RecipientTaxId { get; set; }
    public string? RecipientAttentionTo { get; set; }
    public string? RecipientPhone { get; set; }
    public string? RecipientEmail { get; set; }

    public ShippingWay ShippingWay { get; set; } = ShippingWay.Aereo;
    public ServiceType ServiceType { get; set; } = ServiceType.Economico;
    public DateTime? DeliveryDate { get; set; }

    [Required(ErrorMessage = "El propósito del envío es requerido")]
    public string ShipmentPurpose { get; set; } = string.Empty;

    public InvoiceType InvoiceType { get; set; } = InvoiceType.Comercial;
    public OperationType OperationType { get; set; } = OperationType.A1;

    // El solicitante ya no elige el Incoterm: marca el checklist de tramos de
    // flete aplicables y Trade Staff determina el Incoterm en su revisión.
    public bool RequiresLocalOriginFreight { get; set; }
    public bool RequiresLocalDestinationFreight { get; set; }
    public bool RequiresInternationalFreight { get; set; }
    public string? Incoterm { get; set; }

    public string? InvoiceNumber { get; set; }  // solo aplica cuando Scope == Internacional y Direction == Import
    // HasCOO / documento COO ya no los llena el solicitante: los determina y sube Trade Staff en su revisión.

    public bool IsStackable { get; set; }
    public PaymentResponsibility PaymentResponsibility { get; set; } = PaymentResponsibility.Meax;
    public string? ParcelAccount { get; set; }
    public ParcelCarrier? Carrier { get; set; }  // solo aplica cuando PaymentResponsibility == Receiver
    public bool HasInsurance { get; set; }

    public string? RequestorObservations { get; set; }
    public string? MaterialBajaFolio { get; set; }
    public string? FolioJustification { get; set; }  // sin [Required]

    // Evidencias fotográficas obligatorias (un set por solicitud)
    public string? EvidenceNetWeightPhotoPath { get; set; }
    public string? EvidenceGrossWeightPhotoPath { get; set; }
    public string? EvidenceDimensionsPhotoPath { get; set; }
    public string? EvidenceFinalPackagingPhotoPath { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un manager")]
    public string UserManagerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email del manager es requerido")]
    [EmailAddress]
    public string UserManagerEmail { get; set; } = string.Empty;

    public List<PackageViewModel> Packages { get; set; } = new();
}