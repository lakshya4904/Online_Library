using Entities;
using Entities.Members;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Data.Members;

public class BooksRepository : Repository<Books>
{
    public override PagedResults<Books> GetList(Books objBooks, out Exception exError, int currentPage = 1, int pageSize = 10, int sortBy = 1, bool isAsc = true)
    {

        try
        {
            List<SqlParameter> sqlParams = objBooks.ToSqlParamsList(new[] {
                new SqlParameter("@CurrentPage", SqlDbType.Int) { Value = currentPage },
                new SqlParameter("@PageSize", SqlDbType.Int) { Value = pageSize },
                new SqlParameter("@SortBy", SqlDbType.Int) {Value = sortBy},
                new SqlParameter("@IsAsc", SqlDbType.Bit) {Value = isAsc},
                new SqlParameter("@RecordCount", SqlDbType.BigInt) { Direction = ParameterDirection.Output }
            });
            return PageResult.Select("READ_Books", CommandType.StoredProcedure, sqlParams, out exError);
        }
        catch(Exception ex)
        {
            exError = ex;
        }

        return null;
    }

    public override Books FindOne(Books objBooks, out Exception exError)
    {
        try
        {
            List<SqlParameter> sqlParams = objBooks.ToSqlParamsList(new[] {
                    new SqlParameter("@RecordCount", SqlDbType.BigInt) { Direction = ParameterDirection.Output }
                });
            return PageResult.FindOne("READ_Books", CommandType.StoredProcedure, sqlParams, out exError);
        }
        catch(Exception ex)
        {
            exError = ex;
        }
        return null;
    }

    public override dynamic Add(Books objBooks, out Exception exError)
    {
        try
        {
            List<SqlParameter> sqlParams = objBooks.ToSqlParamsList(new[] {
                    new SqlParameter("@OutId", SqlDbType.Int) { Direction = ParameterDirection.Output }
                });
            return PageResult.Add("CREATE_Books", CommandType.StoredProcedure, sqlParams, out exError);
        }
        catch(Exception ex)
        {
            exError = ex;
        }
        return 0;
    }

    public override dynamic Update(Books objBooks, out Exception exError)
    {
        try
        {
            List<SqlParameter> sqlParams = objBooks.ToSqlParamsList();
            return PageResult.Update("UPDATE_Books", CommandType.StoredProcedure, sqlParams, out exError);
        }
        catch(Exception ex)
        {
            exError = ex;
        }
        return 0;
    }


    public override dynamic Delete(Books objBooks, out Exception exError)
    {
        try
        {
            List<SqlParameter> sqlParams = objBooks.ToSqlParamsList();
            return PageResult.Delete("DELETE_Books", CommandType.StoredProcedure, sqlParams, out exError);
        }
        catch(Exception ex)
        {
            exError = ex;
        }
        return 0;
    }
}