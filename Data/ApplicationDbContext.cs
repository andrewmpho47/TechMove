using Microsoft.EntityFrameworkCore;
using TechMove.Models;

namespace TechMove.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }

        public DbSet<Contract> Contracts { get; set; }

        public DbSet<ServiceRequest> ServiceRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ServiceRequest>()
                .Property(s => s.CostUSD)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ServiceRequest>()
                .Property(s => s.CostZAR)
                .HasPrecision(18, 2);
        }
    }
}
