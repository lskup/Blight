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

        public BlightDbContext(DbContextOptions<BlightDbContext> options) : base(options)
        { 
        }

        public DbSet<User> Users { get; set; }
        public DbSet<PhoneNumber> PhoneNumbers { get; set; }
        public DbSet<Role> Roles { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PhoneNumber>()
                .Property(s => s.Prefix)
                .IsRequired();
            modelBuilder.Entity<PhoneNumber>()
                .Property(s => s.Number)
                .IsRequired();
            modelBuilder.Entity<User>()
                .Property(s => s.Email)
                .IsRequired();
            modelBuilder.Entity<Role>()
                .Property(s => s.Name)
                .IsRequired();

            modelBuilder.Entity<Role>()
                .HasData(
                new Role() {Id=1, Name = "User" },
                new Role() {Id=2, Name = "Admin" }
                );
                
            

        }

    }
}
