using Microsoft.EntityFrameworkCore;
using OT.Assessment.Domain.Models;

namespace OT.Assessment.Infrastructure.Database
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Player> Players { get; set; }
        public DbSet<CasinoWager> CasinoWagers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            try
            {
                modelBuilder.Entity<Player>(entity =>
                {
                    entity.HasKey(e => e.AccountId);
                    entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
                });

                modelBuilder.Entity<CasinoWager>(entity =>
                {
                    entity.HasKey(e => e.WagerId);
                    entity.Property(e => e.Theme).HasMaxLength(255);
                    entity.Property(e => e.Provider).HasMaxLength(255);
                    entity.Property(e => e.GameName).HasMaxLength(255);
                    entity.Property(e => e.CountryCode).HasMaxLength(10);
                    entity.Property(e => e.SessionData).HasColumnType("NVARCHAR(MAX)");

                    entity.HasOne(e => e.Player)
                          .WithMany(p => p.CasinoWagers)
                          .HasForeignKey(e => e.AccountId)
                          .OnDelete(DeleteBehavior.Cascade);
                });
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while configuring the model.", ex);
            }
        }
    }
}
