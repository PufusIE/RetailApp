using RADataManagerLibrary.Models;
using System.Collections.Generic;

namespace RADataManagerLibrary.DataAccess
{
    public interface IUserData
    {
        void CreateUser(UserModel user);
        List<UserModel> GetUserById(string Id);
    }
}