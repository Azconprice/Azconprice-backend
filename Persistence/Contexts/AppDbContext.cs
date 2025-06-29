using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Contexts
{
    public class AppDbContext(DbContextOptions options) : IdentityDbContext<User>(options)
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Specialization>()
                .HasOne(s => s.Profession)
                .WithMany(c => c.Specializations)
                .HasForeignKey(s => s.ProfessionId);

            modelBuilder.Entity<Specialization>()
                .HasOne(s => s.Profession)
                .WithMany(p => p.Specializations)
                .HasForeignKey(s => s.ProfessionId);

            modelBuilder.Entity<WorkerProfile>()
                .HasOne(wp => wp.User)
                .WithOne()
                .HasForeignKey<WorkerProfile>(wp => wp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CompanyProfile>()
                .HasOne(p => p.SalesCategory)
                .WithMany(p => p.CompanyProfiles)
                .HasForeignKey(p => p.SalesCategoryId);

            modelBuilder.Entity<Profession>()
                .HasIndex(p => p.Name)
                .IsUnique();

            modelBuilder.Entity<Specialization>()
                .HasIndex(s => s.Name)
                .IsUnique();

            modelBuilder.Entity<SalesCategory>()
               .HasIndex(sc => sc.Name)
               .IsUnique();
        }

        public DbSet<SalesCategory> SalesCategory { get; set; }
        public DbSet<Profession> Professions { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<WorkerSpecialization> WorkerSpecializations { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<WorkerProfile> WorkerProfiles { get; set; }
        public DbSet<CompanyProfile> CompanyProfiles { get; set; }
        public DbSet<AppLog> AppLogs { get; set; }
        public DbSet<ExcelFileRecord> ExcelFileRecords { get; set; }
        public DbSet<OtpVerification> OtpVerifications { get; set; }
    }
}
