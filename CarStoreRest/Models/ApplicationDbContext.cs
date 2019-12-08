using Microsoft.EntityFrameworkCore;

namespace CarStoreRest.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Car> Cars { get; set; }
    }
}
