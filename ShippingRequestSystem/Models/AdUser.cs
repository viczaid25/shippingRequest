namespace ShippingRequestSystem.Models;

public class AdUser
{
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public List<string> Groups { get; set; } = new();

    // Roles del sistema derivados de grupos AD
    public bool IsTradeStaff => Groups.Contains("Trade_Staff");
    public bool IsTradeCoordinator => Groups.Contains("Trade_Coordinator");
    public bool IsTradeManager => Groups.Contains("Trade_Manager");
    public bool IsProcurement => Groups.Contains("Procurement_Manager");
    public bool IsSalesManager => Groups.Contains("Sales_Manager");
    public bool IsFinanceManager => Groups.Contains("Finance_Manager");
    public bool IsPresident => Groups.Contains("President");
}