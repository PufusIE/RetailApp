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

namespace RADataManagerLibrary.DataAccess
{
    public class UserData
    {
        //gets user by GUID id
        public List<UserModel> GetUserById(string Id)
        {
            SqlDataAccess sql = new SqlDataAccess();
            var p = new { Id };

            var output = sql.LoadData<UserModel, dynamic>("dbo.spUserLookup", p, "RAData");

            return output;
        }

        //getting connection string from web.config
        private string GetConnectionString(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }
    }
}
