using Entities.Members;
using Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Members
{
    public class GenresRepository : Repository<Genres>
    {
        public override PagedResults<Genres> GetList(Genres objGenre, out Exception exError, int currentPage = 1, int pageSize = 10, int sortBy = 1, bool isAsc = true)
        {

            try
            {
                List<SqlParameter> sqlParams = objGenre.ToSqlParamsList(new[] {
                new SqlParameter("@CurrentPage", SqlDbType.Int) { Value = currentPage },
                new SqlParameter("@PageSize", SqlDbType.Int) { Value = pageSize },
                new SqlParameter("@SortBy", SqlDbType.Int) {Value = sortBy},
                new SqlParameter("@IsAsc", SqlDbType.Bit) {Value = isAsc},
                new SqlParameter("@RecordCount", SqlDbType.BigInt) { Direction = ParameterDirection.Output }
            });
                return PageResult.Select("READ_Genre", CommandType.StoredProcedure, sqlParams, out exError);
            }
            catch (Exception ex)
            {
                exError = ex;
            }

            return null;
        }

        public override Genres FindOne(Genres objGenre, out Exception exError)
        {
            try
            {
                List<SqlParameter> sqlParams = objGenre.ToSqlParamsList(new[] {
                    new SqlParameter("@RecordCount", SqlDbType.BigInt) { Direction = ParameterDirection.Output }
                });
                return PageResult.FindOne("READ_Genre", CommandType.StoredProcedure, sqlParams, out exError);
            }
            catch (Exception ex)
            {
                exError = ex;
            }
            return null;
        }

        public override dynamic Add(Genres objGenre, out Exception exError)
        {
            try
            {
                List<SqlParameter> sqlParams = objGenre.ToSqlParamsList(new[] {
                    new SqlParameter("@OutId", SqlDbType.Int) { Direction = ParameterDirection.Output }
                });
                return PageResult.Add("CREATE_Genre", CommandType.StoredProcedure, sqlParams, out exError);
            }
            catch (Exception ex)
            {
                exError = ex;
            }
            return 0;
        }

        public override dynamic Update(Genres objGenre, out Exception exError)
        {
            try
            {
                List<SqlParameter> sqlParams = objGenre.ToSqlParamsList();
                return PageResult.Update("UPDATE_Logs", CommandType.StoredProcedure, sqlParams, out exError);
            }
            catch (Exception ex)
            {
                exError = ex;
            }
            return 0;
        }


        public override dynamic Delete(Genres objGenre, out Exception exError)
        {
            try
            {
                List<SqlParameter> sqlParams = objGenre.ToSqlParamsList();
                return PageResult.Delete("DELETE_Genre", CommandType.StoredProcedure, sqlParams, out exError);
            }
            catch (Exception ex)
            {
                exError = ex;
            }
            return 0;
        }
    }
}