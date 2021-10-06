using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace FridgesManagement.Services.Base
{
    public interface IAsyncQueryExecutor
    {
        Task<T> ExecuteQueryAsync<T>(string query, Func<SqlDataReader, T> converter, params SqlParameter[] parameters);
    }
}
