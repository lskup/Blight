using System;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Blight;
using Blight.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Collections;
using System.Collections.Generic;
using Blight.IntegrationTests.Helpers;
using System.Linq;
using Moq;
using Blight.Interfaces;
using Microsoft.Extensions.Logging;


namespace Blight.IntegrationTests
{
    public class UsersControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Blight.Startup> _factory;
        private readonly HttpClient _client;

        public static IEnumerable<object[]> InvalidDtos()
        {
            return new List<object[]> {
                new object[]{new RegisterUserDto{FirstName="",LastName ="tester1",Email = "tester1@example.com", Password="Tester123!"} },
                new object[]{new RegisterUserDto{FirstName="tester2",LastName ="",Email = "tester2@example.com", Password="Tester123!"} },
                new object[]{new RegisterUserDto{FirstName="tester3",LastName ="tester3",Email = "tester3example.com", Password="Tester123!"} },
                new object[]{new RegisterUserDto{FirstName="tester4",LastName ="tester4",Email = "tester4@example.com", Password="tester"} },
            };
        }

        public UsersControllerTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services =>
                    {
                        services.AddAuthentication("Test")
                            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                                "Test", options => { });
                    });
                });
            _client = _factory.CreateClient();
        }

        [Theory]
        [MemberData(nameof(InvalidDtos))]
        public async Task Post_InvalidObject_BadRequest(RegisterUserDto dto)
        {
            //Arrange
            //_client.DefaultRequestHeaders.Authorization =
            //    new AuthenticationHeaderValue("Test");

            HttpContent httpContent = dto.ToJsonHttpContent();

            //Act
            var result = await _client.PostAsync("api/Users/register", httpContent);

            //Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task Login_ValidUserObject_ReturnsOk()
        {
            //Arrange
            Mock<ISchemeGenerator> stubJwt = new Mock<ISchemeGenerator>();
            stubJwt.SetReturnsDefault("jwt");

            LoginUserDto userExample = new LoginUserDto
            {
                Email = "test@example.com",
                Password = "Test123!",
            };
            HttpContent httpContent = userExample.ToJsonHttpContent();

            //Act
            var result = await _client.PostAsync("api/Users/login", httpContent);

            //Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
        }

    }
}
