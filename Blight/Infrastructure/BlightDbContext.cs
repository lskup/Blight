using Blight.Entieties;
using Microsoft.AspNetCore.Identity;
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


        protected override void OnModelCreating(ModelBuilder builder)
        {
            //modelBuilder.Entity<PhoneNumber>()
            //    .Property(s => s.Prefix)
            //    .IsRequired();
            //modelBuilder.Entity<PhoneNumber>()
            //    .Property(s => s.Number)
            //    .IsRequired();
            //modelBuilder.Entity<User>()
            //    .Property(s => s.Email)
            //    .IsRequired();
            //modelBuilder.Entity<Role>()
            //    .Property(s => s.Name)
            //    .IsRequired();

            //modelBuilder.Entity<Role>()
            //    .HasData(
            //    new Role() {Id=1, Name = "User" },
            //    new Role() {Id=2, Name = "Admin" }
            //    );

            base.OnModelCreating(builder);
            this.SeedRoles(builder);
            this.SeedUsers(builder);

        }
        private void SeedRoles(ModelBuilder builder)
        {
            var roles = new List<Role>()
            {
                new Role()
                {
                    Id = 1,
                    Name = "Master"
                },
                new Role()
                {
                    Id = 2,
                Name = "Admin"
            },
                new Role()
                {
                    Id = 3,
                    Name = "User"
                },
            };

            builder.Entity<Role>().HasData(roles);
        }

        private void SeedUsers(ModelBuilder builder)
        {
            IPasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

            var users = new List<User>();

            var master = new User
            {
                Id = 1,
                FirstName = "Master",
                LastName = "Master",
                Email = "master@example.com",
                RoleId = 1,
                BlockedNumbers = { },
                DateOfBirth = DateTime.Parse("1999-01-01"),
                Nationality = "Poland"
            };

            var admin = new User
            {
                Id = 2,
                FirstName = "Admin",
                LastName = "Admin",
                Email = "admin@example.com",
                RoleId = 2,
                BlockedNumbers = { },
                DateOfBirth = DateTime.Parse("1999-01-01"),
                Nationality = "Poland"

            };

            var user = new User
            {
                Id = 3,
                FirstName = "User",
                LastName = "User",
                Email = "user@example.com",
                RoleId = 3,
                BlockedNumbers = { },
                DateOfBirth = DateTime.Parse("1999-01-01"),
                Nationality = "Poland"

            };

            master.Password = _passwordHasher.HashPassword(master, "Master123!");
            admin.Password = _passwordHasher.HashPassword(admin, "Admin123!");
            user.Password = _passwordHasher.HashPassword(user, "User123!");

            users.Add(master);
            users.Add(admin);
            users.Add(user);

            builder.Entity<User>().HasData(users);
        }

    }
}
