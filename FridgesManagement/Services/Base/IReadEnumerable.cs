using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace FridgesManagement.Services.Base
{
    public interface IReadEnumerable
    {
        IEnumerable<T> Read<T>(SqlDataReader reader, Func<SqlDataReader, T> converter);
    }
}
