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

        public Mock<IGenericRepository<User>> mock = new Mock<IGenericRepository<User>>();

        //Cel - mo¿liwoœæ przekazania predykaty jako parametr testu |==>[MemberData(nameof(predicates_GetAllMethod))]
        public static TheoryData<Expression<Func<User, bool>>> predicates_GetAllMethod = new TheoryData<Expression<Func<User, bool>>>()
        {
            {x=>x.RoleId ==2 },
            {c=>c.Id <=2 },
            {k=>k.FirstName=="testName1" },
        };

        [Fact]
        public async Task GetAll_PredicateIsNull_ListGetAllUserViewModel()
        {
            //Arrange  
            var _dbContext = await InMemoryDataBaseFixture.GetNewDataBaseContext();
            UserRepos userRepos = new UserRepos(_dbContext, null, null,null,null);

            //Act  
            var users = await userRepos.GetAll(null) as List<GetAllUserViewModel> ;

            //Assert  
            Assert.NotNull(users);
            Assert.Equal(3, users.Count());
            Assert.Equal("testName1", users.First().FirstName);
        }

        [Theory]
        [MemberData(nameof(predicates_GetAllMethod))]
        public async Task GetAll_RandomPredicate_ListOfUsers(Expression<Func<User, bool>>? predicate)
        {
            //Arrange  
            var _dbContext = await InMemoryDataBaseFixture.GetNewDataBaseContext();
            UserRepos userRepos = new UserRepos(_dbContext, null, null,null,null);

            //Act  
            var users = await userRepos.GetAll(predicate);

            //Assert  
            Assert.NotNull(users);
            Assert.IsType(typeof(List<User>), users);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task GetById_ExistingId_GetByIdUserViewModel(int id)
        {
            // Arrange
            var _dbContext = await InMemoryDataBaseFixture.GetNewDataBaseContext();
            UserRepos userRepos = new UserRepos(_dbContext, null, null,null,null);

            // Act
            var user = await userRepos.GetById(id) as GetByIdUserViewModel;

            // Assert
            Assert.NotNull(user);
            Assert.Equal($"testName{id}", user.FirstName);
        }

        [Theory]
        [InlineData(6)]
        [InlineData(12)]
        public async Task GetById_NotExistId_NotFoundException(int id)
        {
            // Arrange
            var _dbContext = await InMemoryDataBaseFixture.GetNewDataBaseContext();
            UserRepos userRepos = new UserRepos(_dbContext, null, null,null,null);

            // Act
            var action = async () => await userRepos.GetById(id);

            // Assert
            var caughtException = Assert.ThrowsAsync<NotFoundException>(action);
            Assert.Equal("Element not found", caughtException.Result.Message);
        }

        [Fact]
        public async Task Create_MapperThrowsNewUser_ReturnNewUser()
        {
            //Parametr metody Create (IDto dto) jest mapowany w ciele metody na obiekt User. Mapper jest zdublowany i zwraca konkretny obiekt User,
            //dlatego nie ma sensu przekazywanie obiektu (dto) jako parametr testowy.

            // Arrange
            var _dbContext = await InMemoryDataBaseFixture.GetNewDataBaseContext();
            var mapper = new Mock<IMapper>();
            var hasher = new Mock<IPasswordHasher<User>>();
           
            mapper.SetReturnsDefault(new User()
            {
                FirstName = "testUser0",
                LastName = "testUser0",
                Email = "test0@user.com",
                Password = "tester"
            });
            hasher.SetReturnsDefault<string>("sd33454cecreds");

            UserRepos userRepos = new UserRepos(_dbContext, mapper.Object,hasher.Object,null,null);

            // Act
            var user = await userRepos.Create(null);

            // Assert
            Assert.NotNull(user);
            Assert.Equal("testUser0", user.FirstName);
            Assert.Equal(4, _dbContext.Users.Count());
            Assert.Equal("testUser0", _dbContext.Users.Last().FirstName);
        }

        [Fact]
        public async Task Create_AdminPassword_ReturnNewUserRoleId_2()
        {
            //Parametr metody Create (IDto dto) jest mapowany w ciele metody na obiekt User. Mapper jest zdublowany i zwraca konkretny obiekt User,
            //dlatego nie ma sensu przekazywanie obiektu (dto) jako parametr testowy.

            // Arrange
            var _dbContext = await InMemoryDataBaseFixture.GetNewDataBaseContext();
            var mapper = new Mock<IMapper>();
            var hasher = new Mock<IPasswordHasher<User>>();

            mapper.SetReturnsDefault(new User()
            {
                FirstName = "admin",
                LastName = "admin",
                Email = "admin@user.com",
                Password = "Admin123!"
            });
            hasher.SetReturnsDefault<string>("sd33454cecreds3df");

            UserRepos userRepos = new UserRepos(_dbContext, mapper.Object, hasher.Object, null,null);

            // Act
            var user = await userRepos.Create(null);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(2, user.RoleId);
        }

        [Fact]
        public async Task Delete_ExistedId_DbElementsNumber()
        {
            // Arrange
            var _dbContext = await InMemoryDataBaseFixture.GetNewDataBaseContext();

            GenericRepository<User> userRepos = new GenericRepository<User>(_dbContext,null);

            // Act
            await userRepos.Delete(1);

            // Assert
            Assert.Equal(2, _dbContext.Users.Count());
        }

        [Fact]
        public async Task Update_PassingExistingIdAndUserInfo_UpdatedUser()
        {
            // Arrange
            var _dbContext = await InMemoryDataBaseFixture.GetNewDataBaseContext();
            var userDto = new UpdateUserDto()
            {
                FirstName = "Martin",
                LastName = "Tester",
                Nationality = "Germany"
            };
            UserRepos userRepos = new UserRepos(_dbContext, null,null,null,null);

            // Act
            var updatedUser = await userRepos.Update(1,userDto);

            // Assert
            Assert.NotNull(updatedUser);
            Assert.Equal("Martin", updatedUser.FirstName);
            Assert.Equal("Martin", _dbContext.Users.First().FirstName);
        }

        [Fact]
        public async Task Update_NotExistingId_NotFoundException()
        {
            // Arrange
            var _dbContext = await InMemoryDataBaseFixture.GetNewDataBaseContext();
            var userDto = new UpdateUserDto()
            {
                FirstName = "Martin",
                LastName = "Tester",
                Nationality = "Germany"
            };
            UserRepos userRepos = new UserRepos(_dbContext, null, null,null,null);

            // Act
            var action =async() => await userRepos.Update(10, userDto);

            // Assert
            var caughtException = Assert.ThrowsAsync<NotFoundException>(action);
            Assert.Equal("User not found", caughtException.Result.Message);
        }

    }
}
