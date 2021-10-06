using FridgesManagement.Services.Base;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace FridgesManagement.Services
{
    public class ReadEnumerable : IReadEnumerable
    {
        public IEnumerable<T> Read<T>(SqlDataReader reader, Func<SqlDataReader, T> converter)
        {
            if (!reader.HasRows)
                yield break;
            
            while (reader.Read())
                yield return converter(reader);
        }
    }
}
