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
            UserData sql = new UserData();

            var p = new { Id };

            var output = sql.LoadData1<UserModel, dynamic>("dbo.spUserLookup", p, "RAData");

            return output;
        }

        //getting connection string from web.config
        public string GetConnectionString(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }

        //loads data from database
        public List<T> LoadData1<T, U>(string storedProcedure, U parameters, string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                List<T> rows = connection.Query<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure).ToList();

                return rows;
            }
        }
    }
}
