
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebSystemOne.Models;

namespace WebSystemOne.Data
{
    public class ApplicationDb : IdentityDbContext<ApplicationUserModel>
    {
        public ApplicationDb(DbContextOptions<ApplicationDb> options)
        : base(options)
        {
        }

        public DbSet<AplicationModel> Aplication { get; set; }
        public DbSet<StatusModel> Status { get; set; }
        public DbSet<ServiceModel> Service { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AplicationModel>()
                .HasOne(a => a.Service)
                .WithMany()
                .HasForeignKey(a => a.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AplicationModel>()
                .HasOne(a => a.Status)
                .WithMany()
                .HasForeignKey(a => a.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AplicationModel>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
