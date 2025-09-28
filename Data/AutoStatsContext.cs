using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AutoStats.Models;

namespace AutoStats.Data
{
    public class AutoStatsContext : IdentityDbContext<AutoStatsUser>
    {
        public AutoStatsContext(DbContextOptions<AutoStatsContext> options)
            : base(options)
        {
        }

        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<ServiceRecord> ServiceRecords { get; set; }

        // Dodaj druge DbSet<> entitete npr ako ih imamo (npr. Servise, Gume, itd.)
    }
}
