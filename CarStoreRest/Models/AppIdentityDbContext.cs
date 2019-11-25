using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarStoreRest.Models;
using Microsoft.EntityFrameworkCore;


namespace CarStoreWeb.Models
{
    public class AppIdentityDbContext : DbContext
    {
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}
