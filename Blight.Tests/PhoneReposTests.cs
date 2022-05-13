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
using Moq;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using Blight.Exceptions;
using Microsoft.AspNetCore.Identity;
using Blight.Models;

namespace Blight.UnitTests
{
    public class PhoneReposTests
    {
        //[Fact]
        //public async Task GetAll_PredicateIsNull_ListPhoneNumberViewModel()
        //{
        //    //Arrange  
        //    var _dbContext = await InMemoryDataBaseFixture.GetNewDataBaseContext();
        //    PhoneRepos phoneRepos = new PhoneRepos(_dbContext, null,null);

        //    //Act  
        //    var numbers = await phoneRepos.GetAll(null) as List<PhoneNumberViewModel>;

        //    //Assert  
        //    Assert.NotNull(numbers);
        //    Assert.Equal(3, numbers.Count());
        //    Assert.Equal("123456789", numbers.First().Number);
        //}


        //[Theory]
        //[InlineData(1)]
        //[InlineData(2)]
        //public async Task GetById_ExistingId_PhoneNumberViewModel(int id)
        //{
        //    // Arrange
        //    var _dbContext = await InMemoryDataBaseFixture.GetNewDataBaseContext();
        //    GenericRepository<PhoneNumber> phoneRepos = new GenericRepository<PhoneNumber>(_dbContext, null);

        //    //Act  
        //    var number = await phoneRepos.GetById(id) as PhoneNumberViewModel;

        //    //Assert  
        //    Assert.NotNull(number);
        //    Assert.Equal($"4{id}", number.Prefix);
        //}

        [Theory]
        [InlineData(6)]
        [InlineData(12)]
        public async Task GetById_NotExistedId_NotFoundException(int id)
        {
            // Arrange
            var _dbContext = await InMemoryDataBaseFixture.GetNewDataBaseContext();
            PhoneRepos phoneRepos = new PhoneRepos(_dbContext, null,null);

            // Act
            var action = async () => await phoneRepos.GetById(id);

            // Assert
            var caughtException = Assert.ThrowsAsync<NotFoundException>(action);
            Assert.Equal("Number not found", caughtException.Result.Message);
        }

        [Fact]
        public async Task Create_NewNumber_ReturnsNewNumber()
        {
            // Arrange
            var _dbContext = await InMemoryDataBaseFixture.GetNewDataBaseContext();
            var stubbedMapper = new Mock<IMapper>();
            var stubbedUser = new Mock<IUserContextService>();

            stubbedMapper.SetReturnsDefault(new PhoneNumber()
            {
                Prefix = "48",
                Number = "987654321",
                Users = new List<User>(),
            });

            stubbedUser.Setup(x => x.GetUserId)
                .Returns(1);

            PhoneRepos phoneRepos = new PhoneRepos(_dbContext, stubbedMapper.Object,stubbedUser.Object);

            // Act
            var result = await phoneRepos.Create(null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("987654321", result.Number);
        }

        [Fact]
        public async Task Create_UserAlreadyBlockThisNumber_ForbiddenException()
        {
            // Arrange
            var _dbContext = await InMemoryDataBaseFixture.GetNewDataBaseContext();
            var mapper = new Mock<IMapper>();
            var stubbedUser = new Mock<IUserContextService>();


            var existingNumber = _dbContext.PhoneNumbers.First();

            mapper.SetReturnsDefault(existingNumber);
            stubbedUser.Setup(x => x.GetUserId)
                       .Returns(1);

            PhoneRepos phoneRepos = new PhoneRepos(_dbContext, mapper.Object,stubbedUser.Object);

            // Act
            var action = async ()=> await phoneRepos.Create(null);

            // Assert
            var caughtException = Assert.ThrowsAsync<ForbiddenException>(action);
            Assert.Equal("You already block this number", caughtException.Result.Message);
        }

        [Fact]
        public async Task Create_UserBanned_ForbiddenException()
        {
            // Arrange
            var _dbContext = await InMemoryDataBaseFixture.GetNewDataBaseContext();
            var mapper = new Mock<IMapper>();
            var stubbedUser = new Mock<IUserContextService>();

            var existingNumber = _dbContext.PhoneNumbers.First();

            mapper.SetReturnsDefault(existingNumber);
            stubbedUser.Setup(x => x.GetUserId)
                       .Returns(2);

            PhoneRepos phoneRepos = new PhoneRepos(_dbContext, mapper.Object, stubbedUser.Object);

            // Act
            var action = async () => await phoneRepos.Create(null);

            // Assert
            var caughtException = Assert.ThrowsAsync<ForbiddenException>(action);
            Assert.Equal("You are banned, contact with administration", caughtException.Result.Message);
        }

        [Fact]
        public async Task Delete_UserTriesDeleteNotOwnedNumber_ForbiddendException()
        {
            // Arrange
            var _dbContext = await InMemoryDataBaseFixture.GetNewDataBaseContext();
            var mapper = new Mock<IMapper>();
            var stubbedUser = new Mock<IUserContextService>();

            var existingNumber = _dbContext.PhoneNumbers.First();

            mapper.SetReturnsDefault(existingNumber);
            stubbedUser.Setup(x => x.GetUserId)
                       .Returns(1);

            PhoneRepos phoneRepos = new PhoneRepos(_dbContext, mapper.Object, stubbedUser.Object);

            // Act
            var action = async () => await phoneRepos.Delete(2);

            // Assert
            var caughtException = Assert.ThrowsAsync<ForbiddenException>(action);
            Assert.Equal("Number not belong to your list", caughtException.Result.Message);
        }

        [Fact]
        public async Task SetIsBullyTreshold_LowerThanIsBully_IsBullyChangingToFalse()
        {
            // Arrange
            var _dbContext = await InMemoryDataBaseFixture.GetNewDataBaseContext();

            PhoneRepos phoneRepos = new PhoneRepos(_dbContext, null, null);

            // Act
            var setNewTreshold = await phoneRepos.SetIsBullyTreshold(4,10);
            var phoneNumber = _dbContext.PhoneNumbers.Find(4);

            // Assert
            Assert.NotNull(phoneNumber);
            Assert.Equal(10, phoneNumber.IsBullyTreshold);
            Assert.Equal(false, phoneNumber.IsBully);

        }
        [Fact]
        public async Task SetIsBullyTreshold_HigherThanIsBully_IsBullyChangingToTrue()
        {
            // Arrange
            var _dbContext = await InMemoryDataBaseFixture.GetNewDataBaseContext();

            PhoneRepos phoneRepos = new PhoneRepos(_dbContext, null, null);

            // Act
            var setNewTreshold = await phoneRepos.SetIsBullyTreshold(4, 1);
            var phoneNumber = _dbContext.PhoneNumbers.Find(4);

            // Assert
            Assert.NotNull(phoneNumber);
            Assert.Equal(1, phoneNumber.IsBullyTreshold);
            Assert.Equal(true, phoneNumber.IsBully);

        }

    }
}
