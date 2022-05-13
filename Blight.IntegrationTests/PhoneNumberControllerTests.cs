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

namespace Blight.IntegrationTests
{
    public class PhoneNumberControllerTests:IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Blight.Startup> _factory;
        private readonly HttpClient _client;

        public static IEnumerable<object[]> InvalidDtos()
        {
            return new List<object[]> {
                new object[]{new PhoneNumberDto{Prefix ="123", Number="123456789"} },
                new object[]{new PhoneNumberDto{Prefix ="22", Number="1234"} },
                new object[]{new PhoneNumberDto{Prefix ="12", Number="12345678901"} },
                new object[]{new PhoneNumberDto{Prefix ="", Number="123456789"} },
                new object[]{new PhoneNumberDto{Prefix ="11", Number=""} },
            };
        }

        public PhoneNumberControllerTests(WebApplicationFactory<Startup> factory)
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
        [InlineData("?PageNumber=1&PageSize=7")]
        [InlineData("?PageNumber=0&PageSize=5")]
        public async Task GetAll_InvalidObject_BadRequest(string queryParams)
        {
            //Arrange
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Test");
            //Act
            var result = await _client.GetAsync($"/api/PhoneNumbers/{queryParams}");

            //Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Theory]
        [InlineData("?PageNumber=1&PageSize=5")]
        [InlineData("?PageNumber=5&PageSize=10")]
        [InlineData("?PageNumber=22&PageSize=15")]
        public async Task GetAll_ValidObject_BadRequest(string queryParams)
        {
            //Arrange
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Test");
            //Act
            var result = await _client.GetAsync($"/api/PhoneNumbers/{queryParams}");

            //Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
        }

        [Theory]
        [MemberData(nameof(InvalidDtos))]
        public async Task Post_InvalidObject_BadRequest(PhoneNumberDto dto)
        {
            //Arrange
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Test");

            HttpContent httpContent = dto.ToJsonHttpContent();

            //Act
            var result = await _client.PostAsync("api/PhoneNumbers", httpContent);

            //Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
        }



    }
}
