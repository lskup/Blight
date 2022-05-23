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
using Blight.Services;
using Microsoft.Extensions.Configuration;

namespace Blight.UnitTests
{
    public class AdminPasswordServiceTests
    {
        [Fact]
        public void GeneratePassword_ReturnsString()
        {
            //Arrange

            //Act
            AdminPasswordService serviceTests = new AdminPasswordService();
            var result = serviceTests.GeneratePassword();

            //Assert
            Assert.NotNull(result);
            Assert.Equal(10, result.Length);

        }

        [Fact]
        public void SavePasswordAsTxt_ReturnsNewPasswordInSelectedFile()
        {
            //Arrange
            
            //Act
            AdminPasswordService passwordService = new AdminPasswordService();
            passwordService.GenerateAndSavePasswordToDirectoryFromAppSettings();
            //Assert
        }

        [Fact]
        public void ReadPasswordFromFile_ReturnsPassword()
        {
            //Arrange

            //Act
            AdminPasswordService passwordService = new AdminPasswordService();
            var password = passwordService.ReadPasswordFromFile();
            //Assert
            Assert.NotNull(password);

        }
    }
}
