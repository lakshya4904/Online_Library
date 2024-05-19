using Entities;
using Entities.Members;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Data.Members;

public class LogsRepository : Repository<Logs>
{
    public override PagedResults<Logs> GetList(Logs objLogs, out Exception exError, int currentPage = 1, int pageSize = 10, int sortBy = 1, bool isAsc = true)
    {

        try
        {
            List<SqlParameter> sqlParams = objLogs.ToSqlParamsList(new[] {
                new SqlParameter("@CurrentPage", SqlDbType.Int) { Value = currentPage },
                new SqlParameter("@PageSize", SqlDbType.Int) { Value = pageSize },
                new SqlParameter("@SortBy", SqlDbType.Int) {Value = sortBy},
                new SqlParameter("@IsAsc", SqlDbType.Bit) {Value = isAsc},
                new SqlParameter("@RecordCount", SqlDbType.BigInt) { Direction = ParameterDirection.Output }
            });
            return PageResult.Select("READ_Logs", CommandType.StoredProcedure, sqlParams, out exError);
        }
        catch(Exception ex)
        {
            exError = ex;
        }

        return null;
    }

    public override Logs FindOne(Logs objLogs, out Exception exError)
    {
        try
        {
            List<SqlParameter> sqlParams = objLogs.ToSqlParamsList(new[] {
                    new SqlParameter("@RecordCount", SqlDbType.BigInt) { Direction = ParameterDirection.Output }
                });
            return PageResult.FindOne("READ_Logs", CommandType.StoredProcedure, sqlParams, out exError);
        }
        catch(Exception ex)
        {
            exError = ex;
        }
        return null;
    }

    public override dynamic Add(Logs objLogs, out Exception exError)
    {
        try
        {
            List<SqlParameter> sqlParams = objLogs.ToSqlParamsList(new[] {
                    new SqlParameter("@OutId", SqlDbType.Int) { Direction = ParameterDirection.Output }
                });
            return PageResult.Add("CREATE_Logs", CommandType.StoredProcedure, sqlParams, out exError);
        }
        catch(Exception ex)
        {
            exError = ex;
        }
        return 0;
    }

    public override dynamic Update(Logs objLogs, out Exception exError)
    {
        try
        {
            List<SqlParameter> sqlParams = objLogs.ToSqlParamsList();
            return PageResult.Update("UPDATE_Logs", CommandType.StoredProcedure, sqlParams, out exError);
        }
        catch(Exception ex)
        {
            exError = ex;
        }
        return 0;
    }


    public override dynamic Delete(Logs objLogs, out Exception exError)
    {
        try
        {
            List<SqlParameter> sqlParams = objLogs.ToSqlParamsList();
            return PageResult.Delete("DELETE_Logs", CommandType.StoredProcedure, sqlParams, out exError);
        }
        catch(Exception ex)
        {
            exError = ex;
        }
        return 0;
    }
}