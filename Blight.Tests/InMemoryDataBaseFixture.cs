using Blight.Entieties;
using Blight.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blight.Tests
{
    public class InMemoryDataBaseFixture
    {

        public static async Task<BlightDbContext> GetNewDataBaseContext()
        {
            //Używam globalnego identyfikatora, aby każdy (async)test używał własnej instancji DbContext.

            var options = new DbContextOptionsBuilder<BlightDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;
            var databaseContext = new BlightDbContext(options);
            databaseContext.Database.EnsureCreated();

            bool userExists = await databaseContext.Users.AnyAsync();
            bool phoneExists = await databaseContext.PhoneNumbers.AnyAsync();

            if (!userExists && !phoneExists)
            {
                for (int i = 1; i < 4; i++)
                {
                    await databaseContext.Users.AddAsync(new User()
                    {
                        FirstName = "testName" + i,
                        LastName = "testName" + i,
                        Email = $"test{i}@test.com",
                        DateOfBirth = DateTime.Parse("1999-12-01"),
                        Nationality = "test",
                        Password = "sad325sadcd5fds5d5",
                        RoleId = i % 2 == 1 ? 1 : 2,
                        BlockedNumbers = new List<PhoneNumber>(),
                        Banned = i%2==0?true:false,

                    });
                    await databaseContext.PhoneNumbers.AddAsync(new PhoneNumber()
                    {
                        Prefix = $"4{i}",
                        Number = "123456789",
                        Users = new List<User>()
                        {
                            databaseContext.Users.Find(i)
                        }

                    }); 
                    await databaseContext.SaveChangesAsync();
                }
            }

            return databaseContext;
        }


    }
}
