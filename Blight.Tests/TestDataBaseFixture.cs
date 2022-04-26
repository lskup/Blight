using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blight.Entieties;
using Blight.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Blight.Tests
{
    public class TestDataBaseFixture
    {
        private const string ConnectionString = @"Server=DESKTOP-BVP3D8A;Database=BlightDB;Trusted_Connection=True;";

        private static readonly object _lock = new();
        private static bool _databaseInitialized;

        public TestDataBaseFixture()
        {
            lock (_lock)
            {
                if (!_databaseInitialized)
                {
                    using (var context = CreateContext())
                    {
                        context.Database.EnsureDeleted();
                        context.Database.EnsureCreated();

                        context.AddRange(
                            new User
                            { FirstName = "testName",
                              LastName = "testName",
                              Email = "test@test.com",
                              Password = "TEST_sd35r4dsfsd3svd43_TEST"
                            },
                            new PhoneNumber
                            {
                                Prefix = "48",
                                Number = "123456789",
                                //IsBully = false,
                                //Notified = 1
                            });

                        context.SaveChanges();
                    }

                    _databaseInitialized = true;
                }
            }
        }

        public BlightDbContext CreateContext()
            => new BlightDbContext(
                new DbContextOptionsBuilder<BlightDbContext>()
                    .UseSqlServer(ConnectionString)
                    .Options);
    }
}
