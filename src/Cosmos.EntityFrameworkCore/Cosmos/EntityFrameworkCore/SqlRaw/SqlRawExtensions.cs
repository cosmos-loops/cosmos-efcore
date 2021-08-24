using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Cosmos.Reflection;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Cosmos.EntityFrameworkCore.SqlRaw
{
    /// <summary>
    /// Cosmos type extensions
    /// </summary>
    public static class SqlRawExtensions
    {
        #region Reflection - Is

        public static bool IsSqlSimpleType(this Type type)
        {
            var t = type;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                t = Nullable.GetUnderlyingType(type);

            return t!.IsPrimitive
                || t == typeof(string)
                || t == typeof(DateTime)
                || t == typeof(DateTimeOffset)
                || t == typeof(TimeSpan)
                || t == typeof(Guid)
                || t == typeof(byte[])
                || t == typeof(char[]);
        }

        #endregion

        #region DatabaseFacade - SqlQuery

        public static ISqlRawQuery<object> SqlQuery(this DatabaseFacade database, Type type, string sqlQuery, params DbParameter[] parameters)
        {
            database.CheckNull(nameof(database));
            var typeOfQuery = typeof(SqlRawQuery<>).MakeGenericType(type);
            return (ISqlRawQuery<object>)Activator.CreateInstance(typeOfQuery, database, sqlQuery, parameters);
        }

        public static ISqlRawQuery<object> SqlQuery(this DatabaseFacade database, string sqlQuery, params DbParameter[] parameters)
        {
            database.CheckNull(nameof(database));
            var typeOfQuery = typeof(SqlRawQuery<>).MakeGenericType(typeof(object));
            return (ISqlRawQuery<object>)Activator.CreateInstance(typeOfQuery, database, sqlQuery, parameters);
        }

        public static ISqlRawQuery<T> SqlQuery<T>(this DatabaseFacade database, string sqlQuery, params DbParameter[] parameters)
        {
            database.CheckNull(nameof(database));
            return new SqlRawQuery<T>(database, sqlQuery, parameters);
        }

        public static ISqlRawQuery<T> SqlQuery<T>(this DatabaseFacade database, string sqlQuery, IEnumerable<DbParameter> parameters)
        {
            database.CheckNull(nameof(database));
            return new SqlRawQuery<T>(database, sqlQuery, parameters.ToArray());
        }

        #endregion

        #region DatabaseFacade - SpQuery

        public static ISqlRawQuery<T> StoredProcedure<T>(this DatabaseFacade database, string storedProcName, params DbParameter[] parameters)
        {
            database.CheckNull(nameof(database));
            return new SpRawQuery<T>(database, storedProcName, parameters);
        }

        public static ISqlRawQuery<T> StoredProcedure<T>(this DatabaseFacade database, string storedProcName, IEnumerable<DbParameter> parameters)
        {
            database.CheckNull(nameof(database));
            return new SpRawQuery<T>(database, storedProcName, parameters.ToArray());
        }

        #endregion

        #region DbDataReader - Schema

        public static IDictionary<string, DbColumn> GetSchema<T>(this DbDataReader reader)
        {
            IDictionary<string, DbColumn> valuePairs;

            if (typeof(T).IsTupleType())
            {
                valuePairs = reader.GetColumnSchema().ToDictionary(key => key.ColumnName.ToLower());
            }
            else
            {
                var props = typeof(T).GetRuntimeProperties();
                valuePairs = reader.GetColumnSchema()
                                   .Where(x => props.Any(y => y.Name.ToLower() == x.ColumnName.ToLower()))
                                   .ToDictionary(key => key.ColumnName.ToLower());
            }

            return valuePairs;
        }

        #endregion

        #region DbDataReader - ToList

        public static IList<T> ToList<T>(this DbDataReader reader)
        {
            var objList = new List<T>();
            var colMapping = reader.GetSchema<T>();

            if (!reader.HasRows)
                return objList;

            while (reader.Read())
                objList.Add(SqlRawHelper.MapObject<T>(reader, colMapping));

            return objList;
        }

        public static async Task<IList<T>> ToListAsync<T>(this DbDataReader reader)
        {
            var objList = new List<T>();
            var colMapping = reader.GetSchema<T>();

            if (!reader.HasRows)
                return objList;

            while (await reader.ReadAsync())
                objList.Add(SqlRawHelper.MapObject<T>(reader, colMapping));

            return objList;
        }

        #endregion

        #region DbDataReader - ToDataTable

        public static DataTable ToDataTable(this DbDataReader reader)
        {
            var objDataTable = new DataTable();
            for (var intCounter = 0; intCounter < reader.FieldCount; ++intCounter)
                objDataTable.Columns.Add(reader.GetName(intCounter), reader.GetFieldType(intCounter)!);

            if (!reader.HasRows)
                return objDataTable;

            objDataTable.BeginLoadData();

            var objValues = new object[reader.FieldCount];

            while (reader.Read())
            {
                reader.GetValues(objValues);
                objDataTable.LoadDataRow(objValues, true);
            }

            objDataTable.EndLoadData();

            return objDataTable;
        }

        public static async Task<DataTable> ToDataTableAsync(this DbDataReader reader)
        {
            var objDataTable = new DataTable();
            for (var intCounter = 0; intCounter < reader.FieldCount; ++intCounter)
                objDataTable.Columns.Add(reader.GetName(intCounter), reader.GetFieldType(intCounter)!);

            if (!reader.HasRows)
                return objDataTable;

            objDataTable.BeginLoadData();

            var objValues = new object[reader.FieldCount];

            while (await reader.ReadAsync())
            {
                reader.GetValues(objValues);
                objDataTable.LoadDataRow(objValues, true);
            }

            objDataTable.EndLoadData();

            return objDataTable;
        }

        #endregion

        #region DbDataReader - FirstOrDefault/SingleOrDefault

        public static T FirstOrDefault<T>(this DbDataReader reader)
        {
            var colMapping = reader.GetSchema<T>();
            if (reader.HasRows)
                while (reader.Read())
                    return SqlRawHelper.MapObject<T>(reader, colMapping);

            return default;
        }

        public static async Task<T> FirstOrDefaultAsync<T>(this DbDataReader reader)
        {
            var colMapping = reader.GetSchema<T>();
            if (reader.HasRows)
                while (await reader.ReadAsync())
                    return SqlRawHelper.MapObject<T>(reader, colMapping);

            return default;
        }

        public static T SingleOrDefault<T>(this DbDataReader reader)
        {
            var colMapping = reader.GetSchema<T>();
            var obj = default(T);
            var hasResult = false;

            if (!reader.HasRows)
                return obj;

            while (reader.Read())
            {
                if (hasResult)
                    throw new InvalidOperationException("Sequence contains more than one matching element");

                obj = SqlRawHelper.MapObject<T>(reader, colMapping);
                hasResult = true;
            }

            return obj;
        }

        public static async Task<T> SingleOrDefaultAsync<T>(this DbDataReader reader)
        {
            var colMapping = reader.GetSchema<T>();
            var obj = default(T);
            var hasResult = false;

            if (!reader.HasRows)
                return obj;

            while (await reader.ReadAsync())
            {
                if (hasResult)
                    throw new InvalidOperationException("Sequence contains more than one matching element");

                obj = SqlRawHelper.MapObject<T>(reader, colMapping);
                hasResult = true;
            }

            return obj;
        }

        #endregion

        #region Queryable - ToSql

        public static string ToSql<TEntity>(this IQueryable<TEntity> query) where TEntity : class
        {
            query.CheckNull(nameof(query));
            return SqlRawHelper.ToSql(query);
        }

        #endregion
    }
}