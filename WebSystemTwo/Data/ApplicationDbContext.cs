using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using WebSystemTwo.Models;

namespace WebSystemTwo.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationModel> Applications { get; set; }

        public DbSet<StatusModel> Statuses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationModel>()
                .HasOne(a => a.Status)
                .WithMany()
                .HasForeignKey(a => a.StatusId);

            modelBuilder.Entity<StatusModel>().HasData(
                new StatusModel
                {
                    Id = 4,
                    StatusCode = 1050,
                    StatusName = "Запрос зарегистрирован",
                    Text = "Присвоен регистрационный № {0} от {1}. Запрос принят к рассмотрению."
                }
            );
        }
    }
}