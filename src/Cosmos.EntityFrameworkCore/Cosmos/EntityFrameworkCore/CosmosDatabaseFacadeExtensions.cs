using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Cosmos.EntityFrameworkCore.Core.RawQuery;

/*
 * Reference to:
 *     https://github.com/PaulARoy/EntityFrameworkCore.RawSQLExtensions
 * Author:
 *     Paul Roy
 */

namespace Cosmos.EntityFrameworkCore
{
    /// <summary>
    /// Cosmos database facade extensions
    /// </summary>
    public static class CosmosDatabaseFacadeExtensions
    {
        public static ISqlQuery<object> SqlQuery(this DatabaseFacade database, Type type, string sqlQuery, params  DbParameter[] parameters)
        {
            var typeOfQuery = typeof(SqlRawQuery<>).MakeGenericType(type);
            return (ISqlQuery<object>)Activator.CreateInstance(typeOfQuery, database, sqlQuery, parameters);
        }

        public static ISqlQuery<object> SqlQuery(this DatabaseFacade database, string sqlQuery, params DbParameter[] parameters)
        {
            var typeOfQuery = typeof(SqlRawQuery<>).MakeGenericType(typeof(object));
            return (ISqlQuery<object>)Activator.CreateInstance(typeOfQuery, database, sqlQuery, parameters);
        }

        public static ISqlQuery<T> SqlQuery<T>(this DatabaseFacade database, string sqlQuery, params DbParameter[] parameters)
        {
            return new SqlRawQuery<T>(database, sqlQuery, parameters);
        }

        public static ISqlQuery<T> SqlQuery<T>(this DatabaseFacade database, string sqlQuery, IEnumerable<DbParameter> parameters)
        {
            return new SqlRawQuery<T>(database, sqlQuery, parameters.ToArray());
        }

        public static ISqlQuery<T> StoredProcedure<T>(this DatabaseFacade database, string storedProcName, params DbParameter[] parameters)
        {
            return new SpRawQuery<T>(database, storedProcName, parameters);
        }

        public static ISqlQuery<T> StoredProcedure<T>(this DatabaseFacade database, string storedProcName, IEnumerable<DbParameter> parameters)
        {
            return new SpRawQuery<T>(database, storedProcName, parameters.ToArray());
        }
    }
}