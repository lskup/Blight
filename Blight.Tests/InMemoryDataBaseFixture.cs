using Blight.Entieties;
using Blight.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blight.UnitTests
{
    public class InMemoryDataBaseFixture
    {

        public static async Task<BlightDbContext> GetNewDataBaseContext()
        {
            //Używam globalnego identyfikatora, aby każdy (async)test używał własnej instancji DbContext.
            //Using GUID to provide unique dbContext instance for every test.

            var options = new DbContextOptionsBuilder<BlightDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;
            var databaseContext = new BlightDbContext(options);
            databaseContext.Database.EnsureCreated();

            bool phoneExists = await databaseContext.PhoneNumbers.AnyAsync();

            if (!phoneExists)
            {
                for (int i = 1; i < 4; i++)
                {
                    await databaseContext.PhoneNumbers.AddAsync(new PhoneNumber()
                    {
                        Prefix = $"4{i}",
                        Number = "123456789",
                        Users = new List<User>()
                        {
                            databaseContext.Users.Find(i)
                        }
                    });
                }
                await databaseContext.PhoneNumbers.AddAsync(
          new PhoneNumber
                {
                    Prefix = "11",
                    Number = "123456789",
                    Users = new List<User>
                    {
                        databaseContext.Users.Find(1),
                        databaseContext.Users.Find(2),
                        databaseContext.Users.Find(3),
                    },
                    IsBullyTreshold = 2,

                });
                //await databaseContext.SaveChangesAsync();
            }
            await databaseContext.Users.AddAsync(new User
            {
                FirstName = "tester",
                LastName = "tester",
                Email = $"test@test.com",
                DateOfBirth = DateTime.Parse("1999-12-01"),
                Nationality = "test",
                Password = "sad325sadcd5fds5d5",
                RoleId = 3,
                BlockedNumbers = new List<PhoneNumber>(),
                Banned = true,

            });
            await databaseContext.SaveChangesAsync();

            return databaseContext;

        }

    }
}
