using Blight.Entieties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blight.Infrastructure
{
    public class BlightDbContext:DbContext
    {
        IConfiguration _configuration;

        public BlightDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<PhoneNumber> PhoneNumbers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

    }
}
