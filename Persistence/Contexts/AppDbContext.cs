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
        }

        DbSet<Profession> Professions { get; set; }
        DbSet<Specialization> Specializations { get; set; }
        DbSet<UserSpecialization> UserSpecializations { get; set; }
        public DbSet<WorkerProfile> WorkerProfiles { get; set; }
        public DbSet<CompanyProfile> CompanyProfiles { get; set; }
        public DbSet<AppLog> AppLogs { get; set; }
    }
}
