using Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;


namespace Data
{
    internal class DataMapper<T> : IDataMapper<T>
    {
        public DataMapper(SqlConnection connection) : base(connection) { }
        public override PagedResults<T> Select(string query, CommandType commandType, List<SqlParameter> parameters, out Exception exError)
        {
            PagedResults<T> resultSet = new();
            exError = null;
            SqlCommand command = new(query, (SqlConnection)Connection);
            try
            {
                if(Connection.State != ConnectionState.Open)
                    Connection.Open();
                command.CommandType = commandType;
                command.Parameters.AddRange(parameters.ToArray());
                using(var reader = command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        var item = Activator.CreateInstance<T>();
                        DataTable columns = reader.GetSchemaTable();
                        foreach(var property in typeof(T).GetProperties())
                        {
                            try
                            {
                                if(columns != null && columns.Select($"ColumnName = '{property.Name}'").Any())
                                {
                                    if(!reader.IsDBNull(reader.GetOrdinal(property.Name)))
                                    {
                                        Type convertTo = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                                        property.SetValue(item, Convert.ChangeType(reader[property.Name], convertTo), null);
                                    }
                                }
                                else
                                {
                                    property.SetValue(item, null);
                                }

                            }
                            catch(Exception ex)
                            {
                                property.SetValue(item, null);
                                exError = ex;
                            }
                        }
                        resultSet.Items.Add(item);
                    }
                }
                int recordCount = command.Parameters.Contains("@RecordCount") ? Convert.ToInt32(command.Parameters["@RecordCount"].Value) : 1;
                resultSet.TotalCount = recordCount;
                resultSet.CurrentPage = parameters.Any(x => x.ParameterName == "@CurrentPage") ? Convert.ToInt32(command.Parameters["@CurrentPage"].Value) : 1;
                resultSet.PageSize = parameters.Any(x => x.ParameterName == "@PageSize") ? Convert.ToInt32(command.Parameters["@PageSize"].Value) : 10;
            }
            catch(InvalidOperationException invalid)
            {
                exError = invalid;
            }
            catch(Exception ex)
            {
                exError = ex;
            }
            return resultSet;
        }
        public override T FindOne(string query, CommandType commandType, List<SqlParameter> parameters, out Exception exError)
        {
            return Select(query, commandType, parameters, out exError).Items.FirstOrDefault();
        }
        private dynamic ExecuteNonQuery(string query, CommandType commandType, List<SqlParameter> parameters, out Exception exError)
        {
            exError = null;
            SqlCommand command = new(query, (SqlConnection)Connection);
            try
            {
                if(Connection.State != ConnectionState.Open)
                    Connection.Open();
                command.CommandType = commandType;
                command.Parameters.AddRange(parameters.ToArray());
                if(command.Parameters.Contains("@OutId"))
                {
                    command.ExecuteNonQuery();
                    return command.Parameters["@OutId"].Value;
                }
                else
                {
                    return command.ExecuteNonQuery();
                }
            }
            catch(InvalidOperationException invalid)
            {
                exError = invalid;
            }
            catch(Exception ex)
            {
                exError = ex;
            }
            return 0;
        }
        public override dynamic Add(string query, CommandType commandType, List<SqlParameter> parameters, out Exception exError)
        {
            return ExecuteNonQuery(query, commandType, parameters, out exError);
        }
        public override dynamic Update(string query, CommandType commandType, List<SqlParameter> parameters, out Exception exError)
        {
            return ExecuteNonQuery(query, commandType, parameters, out exError);
        }
        public override dynamic Delete(string query, CommandType commandType, List<SqlParameter> parameters, out Exception exError)
        {
            return ExecuteNonQuery(query, commandType, parameters, out exError);
        }
    }
}