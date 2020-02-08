using System;
using Xunit;
using urlshortener.Controllers;
using urlshortener.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace tests
{
    public class TestLongToShortURL
    {
      private UrlShortenerController controller;
      public IConfigurationRoot Configuration { get; }

      public TestLongToShortURL() 
      {
        var builder = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables();
        Configuration = builder.Build();

        controller = new UrlShortenerController(Configuration);
      }

        [Fact]
        public void TestInvalidShortUrl() {
            //Arrange
            string shorturl = "34";

            //Act
            var result = controller.ShortUrlToLongUrl(shorturl);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void TestInvalidShortUrlLength() {
            //Arrange
            string shorturl = "ZZZa";

            //Act
            var result = controller.ShortUrlToLongUrl(shorturl);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void TestNotStoredShortUrl() {
            //Arrange
            string shorturl = "ZZcShnwQ";

            //Act
            var result = controller.ShortUrlToLongUrl(shorturl);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }
        
    }
}
