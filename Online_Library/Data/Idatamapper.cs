
using Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
namespace Data
{
    internal abstract class IDataMapper<T> : IDisposable
    {
        public IDbConnection Connection
        {
            get; private set;
        }

        public IDataMapper(IDbConnection connection)
        {
            this.Connection = connection ?? throw new NotImplementedException("Connection is invalid");
        }

        public abstract PagedResults<T> Select(string query, CommandType commandType, List<SqlParameter> parameters, out Exception exError);
        public abstract T FindOne(string query, CommandType commandType, List<SqlParameter> parameters, out Exception exError);
        public abstract dynamic Add(string query, CommandType commandType, List<SqlParameter> parameters, out Exception exError);
        public abstract dynamic Update(string query, CommandType commandType, List<SqlParameter> parameters, out Exception exError);
        public abstract dynamic Delete(string query, CommandType commandType, List<SqlParameter> parameters, out Exception exError);
        public void Dispose()
        {
            if(Connection != null)
            {
                if(Connection.State == ConnectionState.Open)
                    Connection.Close();
                Connection.Dispose();
            }

        }
    }
}
