using System;
using Xunit;
using Blight.Interfaces;
using Blight.Controllers;
using Blight.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Blight.Entieties;
using Blight.Repository;
using AutoMapper;
using AutoFixture.Xunit2;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using Moq.AutoMock;
using Blight.Exceptions;
using Microsoft.AspNetCore.Identity;
using Blight.Models;

namespace Blight.Tests
{
    public class UserReposTests
    {

        private async Task<BlightDbContext> GetDataBaseContext()
        {
            var options = new DbContextOptionsBuilder<BlightDbContext>()
                    .UseInMemoryDatabase("InMemoryBlight")
                    .Options;
            var databaseContext = new BlightDbContext(options);
            databaseContext.Database.EnsureCreated();

            bool userExists = await databaseContext.Users.AnyAsync();
            bool phoneExists = await databaseContext.PhoneNumbers.AnyAsync();

            if(!userExists && !phoneExists)
            {
                for (int i = 1; i < 3; i++)
                {
                    await databaseContext.Users.AddAsync(new User()
                    {
                        FirstName = "testName" + i,
                        LastName = "testName" + i,
                        Email = $"test{i}@test.com",
                        DateOfBirth = DateTime.Parse("1999-12-01"),
                        Nationality = "test",
                        HashedPassword = "sad325sadcd5fds5d5"

                    });
                    await databaseContext.PhoneNumbers.AddAsync(new PhoneNumber()
                    {
                        Prefix = $"4{i}",
                        Number = "123456789",
                        IsBully = false,
                        Notified = 1
                    });
                    await databaseContext.SaveChangesAsync();
                }
            }

            return databaseContext;
        }
        public Mock<IUserRepository> mock = new Mock<IUserRepository>();

        // used with [Theory]
        // [MemberData(nameof(dataExpression))]
        //public static TheoryData<Expression<Func<User, bool>>> dataExpression = new TheoryData<Expression<Func<User, bool>>>()
        //{
        //    {x=>x.RoleId ==2 }
        //};

        [Fact]
        public async void GetAll_PredicateIsNull_ListOfUsers()
        {
            // arrange
            mock.Setup(x => x.GetAll(null))
                .Returns((ListOfUsers()));

            // act
            UsersController usersController = new UsersController(mock.Object);

            var result = await usersController.GetAll();

            // assert

            OkObjectResult objectResult = Assert.IsType<OkObjectResult>(result.Result);
            List<User> userList = Assert.IsType<List<User>>(objectResult.Value);
            Assert.NotNull(userList);
            Assert.Equal(ListOfUsers().Result.Count(), userList.Count());
        }

        //public async void FindElement__Dependend_Of_Given_Predicate__ElementFromDb(Expression<Func<User, bool>> predicate)
        //{
        //    // arrange
        //    mock.Setup(x => x.FindElement(predicate))
        //        .ReturnsAsync(FindUserInsideListOfUsers(predicate).Result);
        //    // act
        //    UsersController usersController = new UsersController(mock.Object);

        //    // assert

        //}

        //[Theory]
        //[InlineData(1)]
        //[InlineData(2)]
        //public async void GetById__Given_Existing_Id__User(int id)
        //{
        //    // arrange
        //    var selectedUser = FindUserInsideListOfUsers(x => x.Id == id);

        //    mock.Setup(x => x.GetById(id))
        //        .Returns(Task.FromResult(selectedUser));

        //    // act
        //    UsersController usersController = new UsersController(mock.Object);
        //    var result = await usersController.Get(id);

        //    // assert

        //    OkObjectResult okObjectResult = Assert.IsType<OkObjectResult>(result.Result);
        //    User user = Assert.IsType<User>(okObjectResult.Value);

        //    Assert.Equal(selectedUser, user);

        //}

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void GetById_GivenId_User(int id)
        {
            // arrange
            var mocker = new AutoMocker();
            var user = FindUserInsideListOfUsers(x=>x.Id ==id);


            mocker.Setup<IUserRepository, Task<User>>
                (s => s.GetById(id))
                .Returns(Task.FromResult(user));

            // act
            var mock = mocker.Get<IUserRepository>();
            var testedUser = mock.GetById(id);

            // assert
            Assert.Equal(user, testedUser.Result);
            Assert.Equal(user.FirstName, testedUser.Result.FirstName);

        }

        [Theory]
        [InlineData(3)]
        [InlineData(10)]
        public async void GetById_IdOutOfRange_ThrowNotFoundException(int id)
        {
            // arrange
            var mocker = new AutoMocker();
            var user = FindUserInsideListOfUsers(x => x.Id == id);

            //mocker.Setup<IUserRepository, Task<User>>
            //    (s => s.GetById(id))
            //    .Returns(Task.FromResult(user));

            // act
            var mock = mocker.Get<IUserRepository>();
            var testedUser = mock.GetById(id);

            // assert
            await Assert.ThrowsAsync<NotFoundException>(() => testedUser);
        }

        private async Task<IEnumerable<User>> ListOfUsers()
        {
            List<User> testUsers = new List<User>
            {
                new User()
                {
                    Id =1,
                    FirstName = "testFirstName1",
                    LastName = "testLastName1",
                    Email = "test1@test.com",
                    HashedPassword = "12df4fdg5gbvbv4"
                },
                new User()
                {
                    Id =2,
                    FirstName = "testFirstName2",
                    LastName = "testLastName2",
                    Email = "test2@test.com",
                    HashedPassword = "12df4fdg5gbvbv4"

                }
            };
        
            return testUsers;
        }

        private  User FindUserInsideListOfUsers(Expression<Func<User, bool>> predicate)
        {
            var testUsers = ListOfUsers();

            var selectedUser = testUsers
                .Result
                .AsQueryable()
                .SingleOrDefault(predicate);
            
            return selectedUser;
        }

    }
}
