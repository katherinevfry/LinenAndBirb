using LinenAndBird.Controllers;
using LinenAndBird.DataAccess;
using Microsoft.Extensions.Configuration;
using System;
using Xunit;

namespace LinenAndBird.Tests
{
    public class HatsControllerTest
    {
        [Fact]
        public void hats_return_all_hats()
        {
            //Arrange
            var controller = new HatsController();

            //Act

            //Assert
        }
    }
}
