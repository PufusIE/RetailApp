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
    public class UserData
    {
        private readonly IConfiguration _config;

        public UserData(IConfiguration config)
        {
            _config = config;
        }

        //gets user by GUID id
        public List<UserModel> GetUserById(string Id)
        {
            SqlDataAccess sql = new SqlDataAccess(_config);
            var p = new { Id };

            var output = sql.LoadData<UserModel, dynamic>("dbo.spUserLookup", p, "RAData");

            return output;
        }
    }
}
