using Microsoft.EntityFrameworkCore;
using WealthTrack.API.Models;

namespace WealthTrack.API.Data
{
    public class WealthTrackDbContext : DbContext
    {
        public WealthTrackDbContext(DbContextOptions<WealthTrackDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Advisor> Advisors { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<AdvisorClient> AdvisorClients { get; set; }
        public DbSet<InvestmentAccount> InvestmentAccounts { get; set; }
        public DbSet<Security> Securities { get; set; }
        public DbSet<Holding> Holdings { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<ClientRequest> ClientRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasOne(u => u.Advisor)
                .WithOne(a => a.User)
                .HasForeignKey<Advisor>(a => a.UserId);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Client)
                .WithOne(c => c.User)
                .HasForeignKey<Client>(c => c.UserId);

            modelBuilder.Entity<AdvisorClient>()
                .HasOne(ac => ac.Advisor)
                .WithMany(a => a.AdvisorClients)
                .HasForeignKey(ac => ac.AdvisorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AdvisorClient>()
                .HasOne(ac => ac.Client)
                .WithMany(c => c.AdvisorClients)
                .HasForeignKey(ac => ac.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InvestmentAccount>()
                .Property(a => a.Balance)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<InvestmentAccount>()
                .Property(a => a.AvailableCash)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Security>()
                .HasIndex(s => s.Ticker)
                .IsUnique();

            modelBuilder.Entity<Security>()
                .Property(s => s.CurrentPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Holding>()
                .Property(h => h.Quantity)
                .HasColumnType("decimal(18,4)");

            modelBuilder.Entity<Holding>()
                .Property(h => h.AverageCostBasis)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Quantity)
                .HasColumnType("decimal(18,4)");

            modelBuilder.Entity<Transaction>()
                .Property(t => t.PricePerShare)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Transaction>()
                .Property(t => t.TotalAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.InvestmentAccount)
                .WithMany(a => a.Transactions)
                .HasForeignKey(t => t.InvestmentAccountId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Security)
                .WithMany(s => s.Transactions)
                .HasForeignKey(t => t.SecurityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ClientRequest>()
                .HasOne(r => r.Client)
                .WithMany(c => c.ClientRequests)
                .HasForeignKey(r => r.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ClientRequest>()
                .HasOne(r => r.Advisor)
                .WithMany()
                .HasForeignKey(r => r.AdvisorId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}