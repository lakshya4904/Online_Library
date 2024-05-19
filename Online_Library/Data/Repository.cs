using Entities;
using System;
using System.Data.SqlClient;

namespace Data
{
    public abstract class Repository<T> : IDisposable
    {
        internal readonly DataMapper<T> PageResult = new DataMapper<T>(new SqlConnection(Config.ConnectionString));
        public abstract dynamic Delete(T obj, out Exception exError);
        public abstract dynamic Update(T obj, out Exception exError);
        public abstract dynamic Add(T obj, out Exception exError);
        public abstract T FindOne(T obj, out Exception exError);
        public abstract PagedResults<T> GetList(T obj, out Exception exError, int currentPage = 1, int pageSize = 10, int sortBy = 1, bool isAsc = true);
        public void Dispose()
        {
            PageResult.Dispose();
        }
    }
}
