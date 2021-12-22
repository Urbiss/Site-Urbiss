using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace Urbiss.Repository
{
    static class DbConnectionExtensions
    {
        public static void AddParameters(this DbCommand command, params object[] parms)
        {
            if (parms != null)
            {
                for (int i = 0; i < parms.Length; i += 2)
                {
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = parms[i].ToString();
                    parameter.Value = parms[i + 1];
                    command.Parameters.Add(parameter);
                }
            }
        }

        public static async Task<TValue> ExecuteScalarAsync<TValue>(this DbConnection connection, TValue defaultValue, string sql, params object[] parms)
        {
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.AddParameters(parms);
            try
            {
                connection.Open();
                var value = await command.ExecuteScalarAsync();
                if ((value == null) || (value == DBNull.Value))
                    return defaultValue;
                else
                    return (TValue)value;
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
