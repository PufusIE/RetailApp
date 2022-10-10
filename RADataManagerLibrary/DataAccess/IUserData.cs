using RADataManagerLibrary.Models;
using System.Collections.Generic;

namespace RADataManagerLibrary.DataAccess
{
    public interface IUserData
    {
        List<UserModel> GetUserById(string Id);
    }
}