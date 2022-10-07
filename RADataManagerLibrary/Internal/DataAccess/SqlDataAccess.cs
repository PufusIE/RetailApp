using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace RADataManagerLibrary.Internal.DataAccess
{
    public class SqlDataAccess : IDisposable
    {
        public SqlDataAccess(IConfiguration config)
        {
            _config = config;
        }
        //Getting connection string from web.config
        public string GetConnectionString(string name)
        {
            return _config.GetConnectionString("RAData");
        }

        //Loads data from database
        public List<T> LoadData<T, U>(string storedProcedure, U parameters, string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                List<T> rows = connection.Query<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure).ToList();

                return rows;
            }
        }

        //Writes to database
        public void SaveData<T>(string storedProcedure, T parameters, string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                connection.Execute(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        //For C# transactions
        private IDbConnection _connection;
        private IDbTransaction _transaction;

        //Start of the transaction, opening the connection
        public void StartTransaction(string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);

            _connection = new SqlConnection(connectionString);
            _connection.Open();

            _transaction = _connection.BeginTransaction();

            isClosed = false;
        }

        public List<T> LoadDataInTransaction<T, U>(string storedProcedure, U parameters)
        {          
            List<T> rows = _connection.Query<T>(storedProcedure,
                                                parameters,
                                                commandType: CommandType.StoredProcedure,
                                                transaction: _transaction).ToList();

            return rows;            
        }

        public void SaveDataInTransaction<T>(string storedProcedure, T parameters)
        {
            _connection.Execute(storedProcedure,
                                parameters,
                                commandType: CommandType.StoredProcedure,
                                transaction: _transaction);
        }

        private bool isClosed = false;
        private readonly IConfiguration _config;

        //Two end of the transaction methods, closing the connection
        public void CommitTransaction()
        {
            _transaction?.Commit();
            _connection?.Close();

            isClosed = true;
        }

        public void RollbackTransaction()
        {
            _transaction.Rollback();
            _connection?.Close();

            isClosed = true;
        }

        //Dispose method in case of error
        public void Dispose()
        {
            if (isClosed == false)
            {
                try
                {
                    CommitTransaction();
                }
                catch 
                {
                    //TODO - log this issue
                }
            }

            _transaction = null;
            _connection = null;
        }
    }
}
