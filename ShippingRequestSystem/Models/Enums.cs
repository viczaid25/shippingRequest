namespace ShippingRequestSystem.Models;

public enum ShipmentDirection { Export, Import }
public enum ShipmentScope { Nacional, Internacional }
public enum NationalShipmentCategory { Compras, Traslado }
public enum ShippingWay { Maritimo, Terrestre, Aereo }
public enum ServiceType { Urgente, Economico }
public enum InvoiceType { Comercial, Traslado }
public enum OperationType { A1, RT }
public enum PaymentResponsibility { Meax, Receiver }
public enum RequestStatus
{
    Draft,
    PendingTradeStaffQuotes,     // Staff sube IGI + cotizaciones
    PendingUserQuoteSelection,   // Usuario elige cotización
    PendingPremiumFreight,       // Usuario llena Premium Freight (si ≥$5000)
    // NOTA: el orden de declaración de aquí en adelante ya NO refleja el orden
    // real del flujo (ver ApprovalService.BuildApprovalFlow) — no se reordenan
    // los valores del enum porque ya hay solicitudes reales guardadas con estos
    // números; el int subyacente de cada nombre no debe cambiar.
    // Cadena formal real: User Manager → Sales Manager (si PF reembolsable por
    // cliente) → Procurement Manager (si PF reembolsable por proveedor) →
    // Finance Manager → President → Trade Staff (aprobación formal) →
    // Trade Manager → Trade Coordinator.
    PendingTradeStaffApproval,
    PendingTradeCoordinator,
    PendingTradeManager,
    PendingUserManager,
    PendingProcurementManager,
    PendingSalesManager,
    PendingFinanceManager,
    PendingPresident,
    Approved,
    Rejected
}
public enum PackageType { Pallet, Box }
public enum PremiumFreightDirection { Import, Export, Local }
public enum PremiumFreightScope { Nacional, America, Europa, Asia, Otro }
public enum PremiumFreightTransport { AirFreight, LandFreight, SeaFreight, Charter, Otro }
public enum PremiumFreightPayment { PaidByMeax, ReimbursableBySupplier, ReimbursableByClient }
// Valores explícitos para que SHL (nunca fue una paquetería real) se pueda
// quitar sin desplazar los números ya guardados en BD para FedEx/UPS/DHL/Otro.
public enum ParcelCarrier { FedEx = 1, UPS = 2, DHL = 3, Otro = 4 }