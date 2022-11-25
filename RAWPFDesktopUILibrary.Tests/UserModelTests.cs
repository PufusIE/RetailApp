
using RAWPFDesktopUILibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWPFDesktopUILibrary.Tests
{
    public class UserModelTests
    {
        [Fact]
        public void RoleList_ShouldReturnExpectedValue()
        {
            string expected = "Admin, Manager";

            var model = new UserModel { Roles = CreateDictionarry() };
            var actual = model.RoleList;

            Assert.Equal(expected, actual);
        }         

        private static Dictionary<string, string> CreateDictionarry()
        {
            var output = new Dictionary<string, string>(); 
            output.Add("123", "Admin"); 
            output.Add("234", "Manager");

            return output;
        }
    }
}
