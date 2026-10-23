namespace WealthTrack.API.DTOs
{
    public class CustomerReportDto
    {
        public decimal TotalPortfolioValue { get; set; }
        public decimal TotalGainLoss { get; set; }
        public decimal TotalGainLossPercent { get; set; }
        public int TotalAccounts { get; set; }
        public List<AccountSummaryDto> Accounts { get; set; } = new List<AccountSummaryDto>();
    }

    public class AccountSummaryDto
    {
        public int AccountId { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public decimal AvailableCash { get; set; }
    }

    public class AdvisorReportDto
    {
        public decimal TotalManagedAssets { get; set; }
        public int TotalClients { get; set; }
        public int PendingRequests { get; set; }
        public List<ClientSummaryDto> Clients { get; set; } = new List<ClientSummaryDto>();
    }

    public class ClientSummaryDto
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public decimal TotalPortfolioValue { get; set; }
        public int AccountCount { get; set; }
    }

    public class AdminReportDto
    {
        public int TotalUsers { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalAdvisors { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        public int TotalAccounts { get; set; }
    }
}
