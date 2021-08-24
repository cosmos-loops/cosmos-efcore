using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using Cosmos.IdUtils;
using Cosmos.Models;
using Cosmos.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace Cosmos.EntityFrameworkCore.SqlRaw
{
    internal static class SqlRawHelper
    {
        #region ToSql

        public static string ToSql<TEntity>(IQueryable<TEntity> query) where TEntity : class
        {
            using var enumerator = query.Provider.Execute<IEnumerable<TEntity>>(query.Expression).GetEnumerator();
            var relationalCommandCache = enumerator.Private("_relationalCommandCache");
            var selectExpression = relationalCommandCache.Private<SelectExpression>("_selectExpression");
            var factory = relationalCommandCache.Private<IQuerySqlGeneratorFactory>("_querySqlGeneratorFactory");

            var sqlGenerator = factory.Create();
            var command = sqlGenerator.GetCommand(selectExpression);

            return command.CommandText;
        }

        private static object Private(this object obj, string privateField) => obj?.GetType().GetField(privateField, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj);
        private static T Private<T>(this object obj, string privateField) => (T)obj?.GetType().GetField(privateField, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj);

        #endregion

        #region ToSql - ToSubQuerySqlForWhereSegment

#pragma warning disable EF1001
        public static string ToSubQuerySqlForWhereSegment<TEntity>(IQueryable<TEntity> queryable, DbContext dbCtx, string aliasSeparator) where TEntity : class, IEntity
        {
            string subQuerySql;
            if (queryable.Provider.Execute<IEnumerable>(queryable.Expression) is not SingleQueryingEnumerable<TEntity> query)
                throw new InvalidOperationException("");

            using (var cmd = query.CreateDbCommand())
                subQuerySql = cmd.CommandText;

            var sqlGenHelper = dbCtx.GetService<ISqlGenerationHelper>();
            var primaryKeyName = dbCtx.Set<TEntity>().GetPrimaryKeyName();
            var quotedPrimaryKeyName = sqlGenHelper.DelimitIdentifier(primaryKeyName); //pkId-->"pdId" on NPgsql

            return $"{quotedPrimaryKeyName} IN(SELECT {quotedPrimaryKeyName} FROM ({subQuerySql}) {aliasSeparator} {_uniqueAlias()} )";

            string _uniqueAlias() => $"V{GuidProvider.CreateRandom():N}";
        }
#pragma warning restore EF1001

        #endregion

        #region DiffSql

        /// <summary>
        /// exclude the oldSQL from newSQL
        /// Diff("abc","abc12")=="12"
        /// </summary>
        /// <param name="oldSql"></param>
        /// <param name="newSql"></param>
        /// <returns></returns>
        public static string DiffSql(string oldSql, string newSql)
        {
            if (!newSql.StartsWith(oldSql))
                throw new ArgumentException("NewSql must contain OldSql and start with OldSql.", nameof(newSql));
            return newSql[oldSql.Length..];
        }

        #endregion

        #region MapObject

        public static T MapObject<T>(DbDataReader reader, IDictionary<string, DbColumn> colMapping)
        {
            if (typeof(T).IsSqlSimpleType())
            {
                return (T)reader.GetValue(0);
            }

            var obj = TypeVisit.CreateInstance<T>();

            if (typeof(T).IsTupleType())
            {
                var fields = typeof(T).GetRuntimeFields().ToArray();
                //https://stackoverflow.com/questions/59000557/valuetuple-set-fields-via-reflection
                object objX = obj;
                for (var i = 0; i < fields.Length; i++)
                {
                    var val = Convert.ChangeType(reader.GetValue(i), fields[i].FieldType);
                    fields[i].SetValue(objX, val == DBNull.Value ? null : val);
                }

                obj = (T)Convert.ChangeType(objX, typeof(T));
            }
            else
            {
                foreach (var prop in typeof(T).GetRuntimeProperties())
                {
                    var propName = prop.Name.ToLower();
                    if (colMapping.ContainsKey(propName))
                    {
                        var val = reader.GetValue(colMapping[propName].ColumnOrdinal!.Value);
                        var type = Nullable.GetUnderlyingType(prop.PropertyType);
                        if (type is { IsEnum: true })
                            val = val == DBNull.Value ? null : Enum.ToObject(type, val);
                        prop.SetValue(obj, val == DBNull.Value ? null : val);
                    }
                    else
                    {
                        prop.SetValue(obj, null);
                    }
                }
            }

            return obj;
        }

        #endregion

        #region GenerateList

        //this method is from source code ef core
        public static void GenerateList<T>(
            IReadOnlyList<T> items,
            IRelationalCommandBuilder sql,
            Action<T> generationAction,
            Action<IRelationalCommandBuilder> joinAction = null)
        {
            joinAction ??= delegate(IRelationalCommandBuilder isb) { isb.Append(", "); };

            for (var i = 0; i < items.Count; i++)
            {
                if (i > 0)
                {
                    joinAction.Invoke(sql);
                }

                generationAction(items[i]);
            }
        }

        #endregion
    }
}