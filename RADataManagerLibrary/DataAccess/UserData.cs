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

        //gets user by GUID id
        public List<UserModel> GetUserById(string Id)
        {
            var output = _sql.LoadData<UserModel, dynamic>("dbo.spUserLookup", new { Id }, "RAData");

            return output;
        }
    }
}
