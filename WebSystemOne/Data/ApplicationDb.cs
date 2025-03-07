
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

    }
}
