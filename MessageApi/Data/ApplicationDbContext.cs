using MessageApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MessageApi.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        public DbSet<ApplicationModel> Application { get; set; }
        public DbSet<StatusModel> Status { get; set; }
    }
}
