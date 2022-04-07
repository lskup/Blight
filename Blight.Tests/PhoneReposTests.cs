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

namespace Blight.Tests
{
    public class PhoneReposTests
    {
        [Fact]
        public async Task GetAll_PredicateIsNull_AllPhoneNumbers()
        {
            //Arrange  
            var _dbContext = await InMemoryDataBaseFixture.GetNewDataBaseContext();
            GenericRepository<PhoneNumber> phoneRepos = new GenericRepository<PhoneNumber>(_dbContext, null);

            //Act  
            var numbers = await phoneRepos.GetAll(null);

            //Assert  
            Assert.NotNull(numbers);
            Assert.Equal(3, numbers.Count());
            Assert.Equal("123456789", numbers.First().Number);
        }


        // Metoda generyczna GetById nie jest nadpisywana przez PhoneRepos oraz UserRepos, dlatego ponowne pokrycie wykonane w ramach treningu.  
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async void GetById_ExistingId_PhoneNumber(int id)
        {
            // Arrange
            var _dbContext = await InMemoryDataBaseFixture.GetNewDataBaseContext();
            GenericRepository<PhoneNumber> phoneRepos = new GenericRepository<PhoneNumber>(_dbContext, null);

            //Act  
            var number = await phoneRepos.GetById(id);

            //Assert  
            Assert.NotNull(number);
            Assert.Equal($"4{id}", number.Prefix);
        }

        [Theory]
        [InlineData(6)]
        [InlineData(12)]
        public async void GetById_NotExistId_NotFoundException(int id)
        {
            // Arrange
            var _dbContext = await InMemoryDataBaseFixture.GetNewDataBaseContext();
            GenericRepository<PhoneNumber> phoneRepos = new GenericRepository<PhoneNumber>(_dbContext, null);

            // Act
            var action = async () => await phoneRepos.GetById(id);

            // Assert
            var caughtException = Assert.ThrowsAsync<NotFoundException>(action);
            Assert.Equal("Element not found", caughtException.Result.Message);
        }

        [Fact]
        public async void Create_NewNumber_ReturnsNewNumber()
        {
            // Arrange
            var _dbContext = await InMemoryDataBaseFixture.GetNewDataBaseContext();
            var mapper = new Mock<IMapper>();
            mapper.SetReturnsDefault(new PhoneNumber()
            {
                Prefix = "48",
                Number = "987654321"
            });

            PhoneRepos phoneRepos = new PhoneRepos(_dbContext, mapper.Object);

            // Act
            var result = await phoneRepos.Create(null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("987654321", result.Number);
            Assert.Equal(4, _dbContext.PhoneNumbers.Count());

        }






    }
}
