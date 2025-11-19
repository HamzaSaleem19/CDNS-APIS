using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CDNSBlazorApp.Models;

namespace CDNSBlazorApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Instituation> Instituations { get; set; }
        public DbSet<Instrument> Instruments { get; set; }
        public DbSet<Series> Series { get; set; }
        public DbSet<SeriesPattern> SeriesPatterns { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure composite keys
            modelBuilder.Entity<Series>()
                .HasKey(s => new { s.SerieId, s.SerieTraNo });

            modelBuilder.Entity<SeriesPattern>()
                .HasKey(sp => new { sp.SeriePatId, sp.SeriePatTraNo });

            modelBuilder.Entity<Subscription>()
                .HasKey(s => new { s.SubscriptionId, s.SubscriptionIdTraNo });

            modelBuilder.Entity<Payment>()
                .HasKey(p => new { p.PaymentId, p.PaymentTraNo });

            // Configure relationships
            modelBuilder.Entity<Instrument>()
                .HasOne(i => i.Instituation)
                .WithMany(inst => inst.Instruments)
                .HasForeignKey(i => i.InstituationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Series>()
                .HasOne(s => s.Instrument)
                .WithMany(i => i.Series)
                .HasForeignKey(s => s.SerieId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SeriesPattern>()
                .HasOne(sp => sp.Instrument)
                .WithMany(i => i.SeriesPatterns)
                .HasForeignKey(sp => sp.SeriePatId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Subscription>()
                .HasOne(s => s.Instrument)
                .WithMany(i => i.Subscriptions)
                .HasForeignKey(s => s.SubscriptionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Instrument)
                .WithMany(i => i.Payments)
                .HasForeignKey(p => p.PaymentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure decimal precision
            modelBuilder.Entity<Instrument>()
                .Property(i => i.Amt)
                .HasPrecision(21, 3);

            modelBuilder.Entity<Instrument>()
                .Property(i => i.AmtDiscounted)
                .HasPrecision(21, 3);

            modelBuilder.Entity<Instrument>()
                .Property(i => i.AmtNet)
                .HasPrecision(21, 3);

            modelBuilder.Entity<Series>()
                .Property(s => s.SerieAmt)
                .HasPrecision(21, 3);

            modelBuilder.Entity<SeriesPattern>()
                .Property(sp => sp.Amt)
                .HasPrecision(21, 3);

            modelBuilder.Entity<Subscription>()
                .Property(s => s.ReceiptsAmount)
                .HasPrecision(21, 3);

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(21, 3);
        }
    }
}
