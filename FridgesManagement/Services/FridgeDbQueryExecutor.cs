using FridgesManagement.Services.Base;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace FridgesManagement.Services
{
    public class FridgeDbQueryExecutor : IDisposable, IAsyncQueryExecutor
    {
        private bool _connectionOpened = false;
        
        private readonly SqlConnection _connection;

        private readonly ILogger<FridgeDbQueryExecutor> _logger;

        public FridgeDbQueryExecutor(IConfiguration configuration, ILogger<FridgeDbQueryExecutor> logger)
        {
            _logger = logger;
            
            _connection = new(configuration.GetSection("ConnectionStrings")["FridgeDB"]);

            try
            {
                _connection.Open();
                _connectionOpened = true;
            }
            catch (Exception)
            {
                _logger.LogError("Such db doesn't exists");
            }
        }

        public async Task<T> ExecuteQueryAsync<T>(string query, Func<SqlDataReader, T> converter, params SqlParameter[] parameters)
        {
            if (_connectionOpened)
                return await Task.Run(() => ExecuteQuery(query, converter, parameters));
            return default;
        }

        private T ExecuteQuery<T>(string query, Func<SqlDataReader, T> converter, params SqlParameter[] parameters)
        {
            SqlCommand command = new(query, _connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            var notNullParams = parameters.Where(parameter => parameter != null)
                                          .ToArray();
            
            if (parameters.Length > 0)
                command.Parameters.AddRange(notNullParams);

            try
            {
                using var reader = command.ExecuteReader();
                return converter(reader);
            }
            catch (SqlException)
            {
                _logger.LogError("Such procedure doesn't exists");
                return default;
            }
        }

        public void Dispose()
        {
            if (_connectionOpened)
                _connection.Close();
        }
    }
}
