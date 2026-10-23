using Microsoft.EntityFrameworkCore;
using WealthTrack.API.Data;
using WealthTrack.API.Models;

namespace WealthTrack.API.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(WealthTrackDbContext context)
        {
            if (await context.Users.AnyAsync())
            {
                return;
            }

            var securities = CreateSecurities();
            await context.Securities.AddRangeAsync(securities);
            await context.SaveChangesAsync();

            var users = CreateUsers();
            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();

            var advisorUser1 = users[1];
            var advisorUser2 = users[2];
            var clientUser1  = users[3];
            var clientUser2  = users[4];
            var clientUser3  = users[5];

            var advisors = CreateAdvisors(advisorUser1.Id, advisorUser2.Id);
            await context.Advisors.AddRangeAsync(advisors);
            await context.SaveChangesAsync();

            var clients = CreateClients(clientUser1.Id, clientUser2.Id, clientUser3.Id);
            await context.Clients.AddRangeAsync(clients);
            await context.SaveChangesAsync();

            var advisorClients = CreateAdvisorClients(advisors[0].Id, advisors[1].Id, clients[0].Id, clients[1].Id, clients[2].Id);
            await context.AdvisorClients.AddRangeAsync(advisorClients);
            await context.SaveChangesAsync();

            var accounts = CreateAccounts(clients[0].Id, clients[1].Id, clients[2].Id);
            await context.InvestmentAccounts.AddRangeAsync(accounts);
            await context.SaveChangesAsync();

            var holdings = CreateHoldings(accounts, securities);
            await context.Holdings.AddRangeAsync(holdings);
            await context.SaveChangesAsync();

            var transactions = CreateTransactions(accounts, securities);
            await context.Transactions.AddRangeAsync(transactions);
            await context.SaveChangesAsync();

            var requests = CreateRequests(clients[0].Id, clients[1].Id, advisors[0].Id);
            await context.ClientRequests.AddRangeAsync(requests);
            await context.SaveChangesAsync();
        }
        private static List<Security> CreateSecurities()
        {
            return new List<Security>
            {
                new Security { Ticker = "WLTH",  Name = "WealthCore Inc",        AssetClass = "Stock", CurrentPrice = 142.50m  },
                new Security { Ticker = "APXF",  Name = "Apex Financial ETF",    AssetClass = "ETF",   CurrentPrice = 98.75m   },
                new Security { Ticker = "RIVG",  Name = "Riviera Growth Co",     AssetClass = "Stock", CurrentPrice = 57.20m   },
                new Security { Ticker = "GBND",  Name = "Global Bond Fund",      AssetClass = "Bond",  CurrentPrice = 105.30m  },
                new Security { Ticker = "PRMT",  Name = "Premier Tech Holdings", AssetClass = "Stock", CurrentPrice = 215.80m  },
                new Security { Ticker = "USTB",  Name = "US Treasury Bond ETF",  AssetClass = "Bond",  CurrentPrice = 99.40m   },
                new Security { Ticker = "INDX",  Name = "Index Market ETF",       AssetClass = "ETF",   CurrentPrice = 312.60m  },
            };
        }

        private static List<User> CreateUsers()
        {
            return new List<User>
            {
                new User { FirstName = "Admin",   LastName = "User",     Email = "admin@wealthtrack.com",    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),   Role = "Admin",   IsActive = true },
                new User { FirstName = "James",   LastName = "Hartwell", Email = "j.hartwell@wealthtrack.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Advisor123!"), Role = "Advisor", IsActive = true },
                new User { FirstName = "Sandra",  LastName = "Mercer",   Email = "s.mercer@wealthtrack.com",   PasswordHash = BCrypt.Net.BCrypt.HashPassword("Advisor123!"), Role = "Advisor", IsActive = true },
                new User { FirstName = "Michael", LastName = "Torres",   Email = "m.torres@wealthtrack.com",   PasswordHash = BCrypt.Net.BCrypt.HashPassword("Client123!"),  Role = "Customer", IsActive = true },
                new User { FirstName = "Laura",   LastName = "Torres",   Email = "l.torres@wealthtrack.com",   PasswordHash = BCrypt.Net.BCrypt.HashPassword("Client123!"),  Role = "Customer", IsActive = true },
                new User { FirstName = "David",   LastName = "Chen",     Email = "d.chen@wealthtrack.com",     PasswordHash = BCrypt.Net.BCrypt.HashPassword("Client123!"),  Role = "Customer", IsActive = true },
            };
        }

        private static List<Advisor> CreateAdvisors(int advisorUserId1, int advisorUserId2)
        {
            return new List<Advisor>
            {
                new Advisor { UserId = advisorUserId1, LicenseNumber = "ADV-10421", Specialization = "Portfolio Management"   },
                new Advisor { UserId = advisorUserId2, LicenseNumber = "ADV-20835", Specialization = "Retirement Planning"     },
            };
        }

        private static List<Client> CreateClients(int clientUserId1, int clientUserId2, int clientUserId3)
        {
            return new List<Client>
            {
                new Client { UserId = clientUserId1, PhoneNumber = "555-0101", Address = "101 Oak Street, Austin TX",   DateOfBirth = new DateTime(1982, 4, 15) },
                new Client { UserId = clientUserId2, PhoneNumber = "555-0102", Address = "101 Oak Street, Austin TX",   DateOfBirth = new DateTime(1985, 7, 22) },
                new Client { UserId = clientUserId3, PhoneNumber = "555-0103", Address = "88 Maple Ave, Denver CO",     DateOfBirth = new DateTime(1990, 11, 3) },
            };
        }

        private static List<AdvisorClient> CreateAdvisorClients(int advisorId1, int advisorId2, int clientId1, int clientId2, int clientId3)
        {
            return new List<AdvisorClient>
            {
                new AdvisorClient { AdvisorId = advisorId1, ClientId = clientId1, AssignedAt = DateTime.UtcNow.AddMonths(-18) },
                new AdvisorClient { AdvisorId = advisorId1, ClientId = clientId2, AssignedAt = DateTime.UtcNow.AddMonths(-18) },
                new AdvisorClient { AdvisorId = advisorId2, ClientId = clientId3, AssignedAt = DateTime.UtcNow.AddMonths(-6)  },
            };
        }

        private static List<InvestmentAccount> CreateAccounts(int clientId1, int clientId2, int clientId3)
        {
            return new List<InvestmentAccount>
            {
                new InvestmentAccount { ClientId = clientId1, AccountNumber = "WT-10001", AccountType = "Brokerage",      Balance = 85000.00m,  AvailableCash = 12000.00m, Status = "Active", OpenedAt = DateTime.UtcNow.AddYears(-3)  },
                new InvestmentAccount { ClientId = clientId1, AccountNumber = "WT-10002", AccountType = "TraditionalIRA",  Balance = 142000.00m, AvailableCash = 5000.00m,  Status = "Active", OpenedAt = DateTime.UtcNow.AddYears(-5)  },
                new InvestmentAccount { ClientId = clientId2, AccountNumber = "WT-10003", AccountType = "RothIRA",         Balance = 63000.00m,  AvailableCash = 8500.00m,  Status = "Active", OpenedAt = DateTime.UtcNow.AddYears(-2)  },
                new InvestmentAccount { ClientId = clientId3, AccountNumber = "WT-10004", AccountType = "Brokerage",      Balance = 47500.00m,  AvailableCash = 9000.00m,  Status = "Active", OpenedAt = DateTime.UtcNow.AddYears(-1)  },
            };
        }

        private static List<Holding> CreateHoldings(List<InvestmentAccount> accounts, List<Security> securities)
        {
            return new List<Holding>
            {
                new Holding { InvestmentAccountId = accounts[0].Id, SecurityId = securities[0].Id, Quantity = 100m, AverageCostBasis = 130.00m },
                new Holding { InvestmentAccountId = accounts[0].Id, SecurityId = securities[1].Id, Quantity = 150m, AverageCostBasis = 90.00m  },
                new Holding { InvestmentAccountId = accounts[0].Id, SecurityId = securities[4].Id, Quantity = 50m,  AverageCostBasis = 200.00m },
                new Holding { InvestmentAccountId = accounts[1].Id, SecurityId = securities[3].Id, Quantity = 200m, AverageCostBasis = 100.00m },
                new Holding { InvestmentAccountId = accounts[1].Id, SecurityId = securities[6].Id, Quantity = 80m,  AverageCostBasis = 295.00m },
                new Holding { InvestmentAccountId = accounts[1].Id, SecurityId = securities[5].Id, Quantity = 120m, AverageCostBasis = 98.00m  },
                new Holding { InvestmentAccountId = accounts[2].Id, SecurityId = securities[2].Id, Quantity = 200m, AverageCostBasis = 50.00m  },
                new Holding { InvestmentAccountId = accounts[2].Id, SecurityId = securities[6].Id, Quantity = 60m,  AverageCostBasis = 300.00m },
                new Holding { InvestmentAccountId = accounts[3].Id, SecurityId = securities[0].Id, Quantity = 80m,  AverageCostBasis = 125.00m },
                new Holding { InvestmentAccountId = accounts[3].Id, SecurityId = securities[1].Id, Quantity = 100m, AverageCostBasis = 95.00m  },
            };
        }

        private static List<Transaction> CreateTransactions(List<InvestmentAccount> accounts, List<Security> securities)
        {
            return new List<Transaction>
            {
                new Transaction { InvestmentAccountId = accounts[0].Id, SecurityId = securities[0].Id, TransactionType = "Buy",        Quantity = 100m, PricePerShare = 130.00m, TotalAmount = 13000.00m, Notes = "Initial position",          TransactionDate = DateTime.UtcNow.AddMonths(-12) },
                new Transaction { InvestmentAccountId = accounts[0].Id, SecurityId = securities[1].Id, TransactionType = "Buy",        Quantity = 150m, PricePerShare = 90.00m,  TotalAmount = 13500.00m, Notes = "Diversification purchase",     TransactionDate = DateTime.UtcNow.AddMonths(-10) },
                new Transaction { InvestmentAccountId = accounts[0].Id, SecurityId = null,             TransactionType = "Deposit",    Quantity = 0m,   PricePerShare = 0m,      TotalAmount = 5000.00m,  Notes = "Monthly contribution",           TransactionDate = DateTime.UtcNow.AddMonths(-3)  },
                new Transaction { InvestmentAccountId = accounts[1].Id, SecurityId = securities[3].Id, TransactionType = "Buy",        Quantity = 200m, PricePerShare = 100.00m, TotalAmount = 20000.00m, Notes = "Bond allocation",              TransactionDate = DateTime.UtcNow.AddMonths(-8)  },
                new Transaction { InvestmentAccountId = accounts[1].Id, SecurityId = null,             TransactionType = "Deposit",    Quantity = 0m,   PricePerShare = 0m,      TotalAmount = 10000.00m, Notes = "Annual IRA contribution",         TransactionDate = DateTime.UtcNow.AddMonths(-6)  },
                new Transaction { InvestmentAccountId = accounts[2].Id, SecurityId = securities[2].Id, TransactionType = "Buy",        Quantity = 200m, PricePerShare = 50.00m,  TotalAmount = 10000.00m, Notes = "Growth position",             TransactionDate = DateTime.UtcNow.AddMonths(-9)  },
                new Transaction { InvestmentAccountId = accounts[2].Id, SecurityId = securities[2].Id, TransactionType = "Sell",       Quantity = 50m,  PricePerShare = 55.00m,  TotalAmount = 2750.00m,  Notes = "Partial profit taking",          TransactionDate = DateTime.UtcNow.AddMonths(-2)  },
                new Transaction { InvestmentAccountId = accounts[3].Id, SecurityId = null,             TransactionType = "Deposit",    Quantity = 0m,   PricePerShare = 0m,      TotalAmount = 15000.00m, Notes = "Account opening deposit",        TransactionDate = DateTime.UtcNow.AddMonths(-12) },
                new Transaction { InvestmentAccountId = accounts[3].Id, SecurityId = securities[0].Id, TransactionType = "Buy",        Quantity = 80m,  PricePerShare = 125.00m, TotalAmount = 10000.00m, Notes = "Core equity purchase",         TransactionDate = DateTime.UtcNow.AddMonths(-11) },
            };
        }


        private static List<ClientRequest> CreateRequests(int clientId1, int clientId2, int advisorId)
        {
            return new List<ClientRequest>
            {
                new ClientRequest { ClientId = clientId1, AdvisorId = advisorId, RequestType = "PortfolioReview",     Subject = "Q4 Portfolio Review",           Message = "I would like to review my portfolio performance for this quarter.",       Status = "Resolved",  AdvisorResponse = "Reviewed your portfolio. Performance is up 8% YTD. Well positioned.", SubmittedAt = DateTime.UtcNow.AddDays(-30), ResolvedAt = DateTime.UtcNow.AddDays(-25) },
                new ClientRequest { ClientId = clientId1, AdvisorId = advisorId, RequestType = "Consultation",        Subject = "Rebalancing Discussion",        Message = "I think my portfolio may need rebalancing. Can we discuss?",                Status = "Pending",   AdvisorResponse = "",                                                                    SubmittedAt = DateTime.UtcNow.AddDays(-2)  },
                new ClientRequest { ClientId = clientId2, AdvisorId = advisorId, RequestType = "AccountAssistance",   Subject = "IRA Contribution Question",     Message = "I want to understand my contribution limits for this year.",              Status = "InProgress",AdvisorResponse = "Looking into current IRA limits for you.",                            SubmittedAt = DateTime.UtcNow.AddDays(-5)  },
            };
        }

    }
}