using System;
using System.Collections.Generic;
using System.Text;
using RADataManagerLibrary.Internal.DataAccess;
using RADataManagerLibrary.Models;
using System.Configuration;
using System.Data;
using Dapper;
using System.Linq;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace RADataManagerLibrary.DataAccess
{
    public class UserData : IUserData
    {
        private readonly ISqlDataAccess _sql;

        public UserData(ISqlDataAccess sql)
        {
            _sql = sql;
        }

        // Gets user by GUID id
        public List<UserModel> GetUserById(string Id)
        {
            var output = _sql.LoadData<UserModel, dynamic>("dbo.spUser_Lookup", new { Id }, "RAData");

            return output;
        }

        public void CreateUser(UserModel user)
        {
            _sql.SaveData("dbo.spUser_Insert", new { user.Id, user.FirstName, user.LastName, user.EmailAddress }, "RAData");
        }
    }
}
