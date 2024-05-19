using Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

#pragma warning disable 1591

namespace Data
{

    [AttributeUsage(AttributeTargets.Property)]
    public class QueryParamNameAttribute : Attribute
    {
        public string Name
        {
            get; set;
        }
        public QueryParamNameAttribute(string name)
        {
            Name = name;
        }
    }

    public static class SqlParameterExtensions
    {
        private class QueryParamInfo
        {
            public string Name
            {
                get; set;
            }
            public object Value
            {
                get; set;
            }
        }

        public static object[] ToSqlParamsArray(this object obj, SqlParameter[] additionalParams = null)
        {
            var result = ToSqlParamsList(obj, additionalParams);
            return result.ToArray<object>();
        }

        public static List<SqlParameter> ToSqlParamsList(this object obj, SqlParameter[] additionalParams = null)
        {
            var props = (
                from p in obj.GetType().GetProperties()
                let value = p.GetValue(obj) ?? DBNull.Value
                let type = p.PropertyType.FullName
                let nameAttr = p.GetCustomAttributes(typeof(QueryParamNameAttribute), true)
                let ignoreAttr = p.GetCustomAttributes(typeof(QueryParamIgnoreAttribute), true)
                select new {
                    Property = p,
                    Names = nameAttr,
                    Ignores = ignoreAttr,
                    Value = value,
                    Type = type
                }).ToList();
            var result = new List<SqlParameter>();
            props.ForEach(p =>
            {
                if(p.Ignores != null && p.Ignores.Length > 0 && (p.Value == DBNull.Value || p.Property.PropertyType.GetGenericTypeDefinition() == typeof(List<>)))
                    return;

                var name = p.Names.FirstOrDefault() as QueryParamNameAttribute;
                var pinfo = new QueryParamInfo
                {
                    Name = !string.IsNullOrWhiteSpace(name?.Name) ? $"@{name.Name}" : $"@{p.Property.Name}",
                    Value = p.Property.GetValue(obj) ?? DBNull.Value
                };

                var sqlParam = new SqlParameter(pinfo.Name, TypeConvertor.ToSqlDbType(p.Property.PropertyType))
                {
                    Value = (p.Type.Contains("String") && pinfo.Value != DBNull.Value)
                        ? (object)pinfo.Value.ToString()
                        : pinfo.Value
                };
                if(sqlParam.Value != DBNull.Value)
                    result.Add(sqlParam);
            });
            if(additionalParams != null && additionalParams.Length > 0)
                result.AddRange(additionalParams);
            return result;
        }
    }

    public static class TypeConvertor
    {
        private struct DbTypeMapEntry
        {
            public readonly Type Type;
            public readonly DbType DbType;
            public readonly SqlDbType SqlDbType;
            public DbTypeMapEntry(Type type, DbType dbType, SqlDbType sqlDbType)
            {
                Type = type;
                DbType = dbType;
                SqlDbType = sqlDbType;
            }

        };

        private static readonly ArrayList DbTypeList = new ArrayList();

        #region Constructors
        static TypeConvertor()
        {
            var dbTypeMapEntry = new DbTypeMapEntry(typeof(bool), DbType.Boolean, SqlDbType.Bit);
            DbTypeList.Add(dbTypeMapEntry);

            dbTypeMapEntry = new DbTypeMapEntry(typeof(byte), DbType.Double, SqlDbType.TinyInt);
            DbTypeList.Add(dbTypeMapEntry);

            dbTypeMapEntry = new DbTypeMapEntry(typeof(byte[]), DbType.Binary, SqlDbType.Image);
            DbTypeList.Add(dbTypeMapEntry);

            dbTypeMapEntry = new DbTypeMapEntry(typeof(DateTime), DbType.DateTime, SqlDbType.DateTime);
            DbTypeList.Add(dbTypeMapEntry);

            dbTypeMapEntry = new DbTypeMapEntry(typeof(decimal), DbType.Decimal, SqlDbType.Decimal);
            DbTypeList.Add(dbTypeMapEntry);

            dbTypeMapEntry = new DbTypeMapEntry(typeof(double), DbType.Double, SqlDbType.Float);
            DbTypeList.Add(dbTypeMapEntry);

            dbTypeMapEntry = new DbTypeMapEntry(typeof(Guid), DbType.Guid, SqlDbType.UniqueIdentifier);
            DbTypeList.Add(dbTypeMapEntry);

            dbTypeMapEntry = new DbTypeMapEntry(typeof(short), DbType.Int16, SqlDbType.SmallInt);
            DbTypeList.Add(dbTypeMapEntry);

            dbTypeMapEntry = new DbTypeMapEntry(typeof(int), DbType.Int32, SqlDbType.Int);
            DbTypeList.Add(dbTypeMapEntry);

            dbTypeMapEntry = new DbTypeMapEntry(typeof(long), DbType.Int64, SqlDbType.BigInt);
            DbTypeList.Add(dbTypeMapEntry);

            dbTypeMapEntry = new DbTypeMapEntry(typeof(object), DbType.Object, SqlDbType.Variant);
            DbTypeList.Add(dbTypeMapEntry);

            dbTypeMapEntry = new DbTypeMapEntry(typeof(string), DbType.String, SqlDbType.VarChar);
            DbTypeList.Add(dbTypeMapEntry);

        }

        #endregion

        #region Methods


        public static Type ToNetType(DbType dbType)
        {
            var entry = Find(dbType);
            return entry.Type;
        }


        public static Type ToNetType(SqlDbType sqlDbType)
        {
            var entry = Find(sqlDbType);
            return entry.Type;
        }


        public static DbType ToDbType(Type type)
        {
            var entry = Find(type);
            return entry.DbType;
        }


        public static DbType ToDbType(SqlDbType sqlDbType)
        {
            var entry = Find(sqlDbType);
            return entry.DbType;
        }


        public static SqlDbType ToSqlDbType(Type type)
        {
            var entry = Find(type);
            return entry.SqlDbType;
        }


        public static SqlDbType ToSqlDbType(DbType dbType)
        {
            var entry = Find(dbType);
            return entry.SqlDbType;
        }

        private static DbTypeMapEntry Find(Type type)
        {
            object retObj = null;
            foreach(var dtType in DbTypeList)
            {
                var entry = (DbTypeMapEntry)dtType;
                if(entry.Type == (Nullable.GetUnderlyingType(type) ?? type))
                {
                    retObj = entry;
                    break;
                }
            }
            if(retObj == null)
            {
                throw
                new ApplicationException("Referenced an unsupported Type " + type);
            }
            return (DbTypeMapEntry)retObj;
        }

        private static DbTypeMapEntry Find(DbType dbType)
        {
            object retObj = null;
            foreach(var dtType in DbTypeList)
            {
                var entry = (DbTypeMapEntry)dtType;
                if(entry.DbType == dbType)
                {
                    retObj = entry;
                    break;
                }
            }
            if(retObj == null)
            {
                throw
                new ApplicationException("Referenced an unsupported DbType " + dbType.ToString());
            }
            return (DbTypeMapEntry)retObj;
        }

        private static DbTypeMapEntry Find(SqlDbType sqlDbType)
        {
            object retObj = null;
            foreach(var dtType in DbTypeList)
            {
                var entry = (DbTypeMapEntry)dtType;
                if(entry.SqlDbType == sqlDbType)
                {
                    retObj = entry;
                    break;
                }
            }
            if(retObj == null)
            {
                throw
                new ApplicationException("Referenced an unsupported SqlDbType");
            }
            return (DbTypeMapEntry)retObj;
        }

        #endregion
    }
}
