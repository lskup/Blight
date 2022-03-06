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

    }
}
