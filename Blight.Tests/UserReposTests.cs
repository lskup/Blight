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
using Microsoft.Extensions.Logging;

namespace Blight.UnitTests
{
    public class UserReposTests
    {

        public Mock<IGenericRepository<User>> mock = new Mock<IGenericRepository<User>>();

        //Purpose - passing predicate as test method parameter|==>[MemberData(nameof(predicates_GetAllMethod))]
        public static TheoryData<Expression<Func<User, bool>>> predicates_GetAllMethod = new TheoryData<Expression<Func<User, bool>>>()
        {
            {x=>x.RoleId ==2 },
            {c=>c.Id <=2 },
            {k=>k.FirstName=="testName1" },
        };

        //[Fact]
        //public async Task GetAll_PredicateIsNull_ListGetAllUserViewModel()
        //{
        //    //Arrange  
        //    var _dbContext = await InMemoryDataBaseFixture.GetNewDataBaseContext();
        //    UserRepos userRepos = new UserRepos(_dbContext, null, null,null,null);

        //    //Act  
        //    var users = await userRepos.GetAll(null) as List<GetAllUserViewModel> ;

        //    //Assert  
        //    Assert.NotNull(users);
        //    Assert.Equal(3, users.Count());
        //    Assert.Equal("testName1", users.First().FirstName);
        //}

        //[Theory]
        //[MemberData(nameof(predicates_GetAllMethod))]
        //public async Task GetAll_RandomPredicate_ListOfUsers(Expression<Func<User, bool>>? predicate)
        //{
        //    //Arrange  
        //    var _dbContext = await InMemoryDataBaseFixture.GetNewDataBaseContext();
        //    UserRepos userRepos = new UserRepos(_dbContext, null, null,null,null);

        //    //Act  
        //    var users = await userRepos.GetAll(predicate);

        //    //Assert  
        //    Assert.NotNull(users);
        //    Assert.IsType(typeof(List<User>), users);
        //}

        //[Theory]
        //[InlineData(1)]
        //[InlineData(2)]
        //public async Task GetById_ExistingId_GetByIdUserViewModel(int id)
        //{
        //    // Arrange
        //    var _dbContext = await InMemoryDataBaseFixture.GetNewDataBaseContext();
        //    UserRepos userRepos = new UserRepos(_dbContext, null, null,null,null);

        //    // Act
        //    var user = await userRepos.GetById(id) as GetByIdUserViewModel;

        //    // Assert
        //    Assert.NotNull(user);
        //    Assert.Equal($"testName{id}", user.FirstName);
        //}

        [Theory]
        [InlineData(2)]
        public async Task GetById_OtherUser_ThrowForbiddenException(int id)
        {
            // Arrange
            var _dbContext = await InMemoryDataBaseFixture.GetNewDataBaseContext();
            var stubbedUser = new Mock<IUserContextService>();


            stubbedUser.Setup(x => x.GetUserId)
                       .Returns(3);

            UserRepos userRepos = new UserRepos(_dbContext, null,null,null,
                    stubbedUser.Object,null,null,null,null,null);

            // Act
            var action = async () => await userRepos.GetById(id);

            // Assert
            var caughtException = Assert.ThrowsAsync<ForbiddenException>(action);
            Assert.Equal("Action forbidden", caughtException.Result.Message);
        }

        [Fact]
        public async Task Create_MapperThrowsNewUser_ReturnNewUser()
        {
            // Arrange
            var _dbContext = await InMemoryDataBaseFixture.GetNewDataBaseContext();
            var mapper = new Mock<IMapper>();
            var hasher = new Mock<IPasswordHasher<User>>();
            var logger = new Mock<ILogger<UserRepos>>();
            var adminService = new Mock<IAdminPasswordService>();

            mapper.SetReturnsDefault(new User()
            {
                FirstName = "testUser0",
                LastName = "testUser0",
                Email = "test0@user.com",
                Password = "tester"
            });
            hasher.SetReturnsDefault<string>("sd33454cecreds");
            logger.SetReturnsDefault<string>("Creating user sample");
            adminService.SetReturnsDefault("Admin123!");

            UserRepos userRepos = new UserRepos(_dbContext, mapper.Object,hasher.Object
                ,null,null,adminService.Object,logger.Object, null, null, null);

            // Act
            var user = await userRepos.Create(null);

            // Assert
            Assert.NotNull(user);
            Assert.Equal("testUser0", user.FirstName);
            Assert.Equal(5, _dbContext.Users.Count());
            Assert.Equal("testUser0", _dbContext.Users.Last().FirstName);
        }

        [Fact]
        public async Task Create_AdminPassword_ReturnNewUserRoleId_2()
        {

            // Arrange
            var _dbContext = await InMemoryDataBaseFixture.GetNewDataBaseContext();
            var mapper = new Mock<IMapper>();
            var hasher = new Mock<IPasswordHasher<User>>();
            var logger = new Mock<ILogger<UserRepos>>();
            var adminService = new Mock<IAdminPasswordService>();

            mapper.SetReturnsDefault(new User()
            {
                FirstName = "admin",
                LastName = "admin",
                Email = "admin@user.com",
                Password = "Admin123!"
            });
            hasher.SetReturnsDefault<string>("sd33454cecreds3df");
            logger.SetReturnsDefault<string>("Creating user sample");
            adminService.SetReturnsDefault("Admin123!");

            UserRepos userRepos = new UserRepos(_dbContext, mapper.Object, hasher.Object,
                null,null,adminService.Object,logger.Object, null, null, null);

            // Act
            var user = await userRepos.Create(null);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(2, user.RoleId);
        }

        [Theory]
        [InlineData(2)]
        public async Task Delete_OtherUser_ThrowForbiddenException(int id)
        {
            // Arrange
            var _dbContext = await InMemoryDataBaseFixture.GetNewDataBaseContext();
            var stubbedUser = new Mock<IUserContextService>();


            stubbedUser.Setup(x => x.GetUserId)
                       .Returns(3);

            UserRepos userRepos = new UserRepos(_dbContext, null, null, null,
                stubbedUser.Object,null,null, null, null, null);

            // Act
            var action = async () => await userRepos.Delete(id);

            // Assert
            var caughtException = Assert.ThrowsAsync<ForbiddenException>(action);
            Assert.Equal("Action forbidden", caughtException.Result.Message);
        }

        [Fact]
        public async Task Update_PassingExistingIdAndUserInfo_UpdatedUser()
        {
            // Arrange
            var _dbContext = await InMemoryDataBaseFixture.GetNewDataBaseContext();
            var stubbedUser = new Mock<IUserContextService>();

            var userDto = new UpdateUserDto()
            {
                FirstName = "Martin",
                LastName = "Tester",
                Nationality = "Germany"
            };
            stubbedUser.Setup(x => x.GetUserId)
                       .Returns(4);


            UserRepos userRepos = new UserRepos(_dbContext, null,null,null,
                stubbedUser.Object,null,null, null, null, null);

            // Act
            var updatedUser = await userRepos.Update(4,userDto) as User;
            

            // Assert
            Assert.NotNull(updatedUser);
            Assert.Equal("Martin", updatedUser.FirstName);
        }

        [Fact]
        public async Task Update_UserForceUpdateOtherUser_ThrowForbiddenException()
        {
            // Arrange
            var _dbContext = await InMemoryDataBaseFixture.GetNewDataBaseContext();
            var stubbedUser = new Mock<IUserContextService>();

            var userDto = new UpdateUserDto()
            {
                FirstName = "Martin",
                LastName = "Tester",
                Nationality = "Germany"
            };
            stubbedUser.Setup(x => x.GetUserId)
                       .Returns(3);

            UserRepos userRepos = new UserRepos(_dbContext, null, null,null,
                stubbedUser.Object,null,null, null, null, null);

            // Act
            var action =async() => await userRepos.Update(10, userDto);

            // Assert
            var caughtException = Assert.ThrowsAsync<ForbiddenException>(action);
            Assert.Equal("Action forbidden", caughtException.Result.Message);
        }

        [Fact]
        public async Task BanUserChange_ExisingUser_UserBanStatusInfo()
        {
            // Arrange
            var _dbContext = await InMemoryDataBaseFixture.GetNewDataBaseContext();
            var stubLogger = LoggerHelper.GetLogger<UserRepos>();

            UserRepos userRepos = new UserRepos(_dbContext, null, null, null,
                null,null,stubLogger.Object, null, null, null);

            // Act
            var result = await userRepos.BanUser_Change(3);

            // Assert
            Assert.NotNull(result);
        }

    }
}
