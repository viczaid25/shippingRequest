using ShippingRequestSystem.Models;

namespace ShippingRequestSystem.Helpers;

public static class EnumExtensions
{
    public static string ToDisplayName(this RequestStatus status) => status switch
    {
        RequestStatus.Draft => "Borrador",
        RequestStatus.PendingTradeStaffQuotes => "Pendiente — Trade Staff (IGI/Cotizaciones)",
        RequestStatus.PendingUserQuoteSelection => "Pendiente — Selección de Cotización",
        RequestStatus.PendingPremiumFreight => "Pendiente — Premium Freight",
        RequestStatus.PendingTradeStaffApproval => "Pendiente — Aprobación Trade Staff",
        RequestStatus.PendingTradeCoordinator => "Pendiente — Trade Coordinator",
        RequestStatus.PendingTradeManager => "Pendiente — Trade Manager",
        RequestStatus.PendingUserManager => "Pendiente — Manager de Usuario",
        RequestStatus.PendingProcurementManager => "Pendiente — Procurement",
        RequestStatus.PendingSalesManager => "Pendiente — Ventas",
        RequestStatus.PendingFinanceManager => "Pendiente — Finanzas",
        RequestStatus.PendingPresident => "Pendiente — Presidencia",
        RequestStatus.Approved => "Aprobado",
        RequestStatus.Rejected => "Rechazado",
        _ => status.ToString()
    };

    public static string ToStatusColor(this RequestStatus status) => status switch
    {
        RequestStatus.Approved => "#16a34a",
        RequestStatus.Rejected => "#dc2626",
        RequestStatus.Draft => "#64748b",
        RequestStatus.PendingUserQuoteSelection => "#7c3aed",
        RequestStatus.PendingPremiumFreight => "#d97706",
        RequestStatus.PendingTradeStaffApproval => "#0369a1",
        _ => "#d97706"
    };

    public static string ToStatusBg(this RequestStatus status) => status switch
    {
        RequestStatus.Approved => "#dcfce7",
        RequestStatus.Rejected => "#fee2e2",
        RequestStatus.Draft => "#f1f5f9",
        RequestStatus.PendingUserQuoteSelection => "#ede9fe",
        RequestStatus.PendingPremiumFreight => "#fef3c7",
        RequestStatus.PendingTradeStaffApproval => "#e0f2fe",
        _ => "#fef3c7"
    };

    public static string ToDisplayName(this ShippingWay way) => way switch
    {
        ShippingWay.Maritimo => "Marítimo",
        ShippingWay.Terrestre => "Terrestre",
        ShippingWay.Aereo => "Aéreo",
        _ => way.ToString()
    };

    public static string ToDisplayName(this ServiceType st) => st switch
    {
        ServiceType.Urgente => "Urgente",
        ServiceType.Economico => "Económico",
        _ => st.ToString()
    };

    public static string ToDisplayName(this PremiumFreightTransport t) => t switch
    {
        PremiumFreightTransport.AirFreight => "Air Freight",
        PremiumFreightTransport.LandFreight => "Land Freight",
        PremiumFreightTransport.SeaFreight => "Sea Freight",
        PremiumFreightTransport.Charter => "Charter",
        PremiumFreightTransport.Otro => "Otro",
        _ => t.ToString()
    };

    public static string ToShipmentIcon(this ShippingRequest r) => r.Scope switch
    {
        ShipmentScope.Internacional => r.Direction == ShipmentDirection.Export ? "📤" : "📥",
        _ => r.NationalCategory == NationalShipmentCategory.Compras ? "🛒" : "🏭"
    };

    public static string ToShipmentLabel(this ShippingRequest r) => r.Scope switch
    {
        ShipmentScope.Internacional => r.Direction == ShipmentDirection.Export ? "EXPORT" : "IMPORT",
        _ => r.NationalCategory?.ToString().ToUpper() ?? "NACIONAL"
    };

    public static string ToDisplayName(this PremiumFreightPayment p) => p switch
    {
        PremiumFreightPayment.PaidByMeax => "Pagado por Meax",
        PremiumFreightPayment.ReimbursableBySupplier => "Reembolsable por Proveedor",
        PremiumFreightPayment.ReimbursableByClient => "Reembolsable por Cliente",
        _ => p.ToString()
    };
}