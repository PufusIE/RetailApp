using RAWPFDesktopUILibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWPFDesktopUILibrary.Tests
{
    public class LogedInUserModelTests
    {
        [Fact]
        public void ResetUserModel_ShouldResetLoggedInUserModel()
        {
            var expected = new LoggedInUserModel
            {
                Token = "",
                Id = "",
                FirstName = "",
                LastName = "",
                EmailAddress = "",
                CreateDate = DateTime.MinValue
            };

            var actual = new LoggedInUserModel
            {
                Token = "1",
                Id = "1",
                FirstName = "a",
                LastName = "a",
                EmailAddress = "a",
                CreateDate = DateTime.Now
            };
            actual.ResetUserModel();

            Assert.Equal(expected.Token, actual.Token);
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.FirstName, actual.FirstName);
            Assert.Equal(expected.LastName, actual.LastName);
            Assert.Equal(expected.EmailAddress, actual.EmailAddress);
            Assert.Equal(expected.CreateDate, actual.CreateDate);
        }
    }
}
