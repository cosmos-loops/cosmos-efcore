using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Cosmos.EntityFrameworkCore.Internals;
using Cosmos.Models;
using Cosmos.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

// ReSharper disable EqualExpressionComparison

namespace Cosmos.EntityFrameworkCore.SqlRaw
{
    /*
     * Reference: Zack.EFCore.Batch
     *   url: https://github.com/yangzhongke/Zack.EFCore.Batch
     *   author: 杨中科
     */
    
    internal static class SqlRawDeleteCommandGenerator
    {
        public static string Generate<TEntity>(
            DbContext ctx, Expression<Func<TEntity, bool>> predicate, bool ignoreQueryFilters,
            out IReadOnlyDictionary<string, object> parameters)
            where TEntity : class, IEntity
        {
            return Generate(ctx, ctx.Set<TEntity>(), predicate, ignoreQueryFilters, out parameters);
        }

        public static string Generate<TEntity>(
            DbContext ctx, IQueryable<TEntity> dbSet,
            Expression<Func<TEntity, bool>> predicate, bool ignoreQueryFilters,
            out IReadOnlyDictionary<string, object> parameters)
            where TEntity : class, IEntity
        {
            var sqlBuilder = new StringBuilder();

            var queryable = dbSet.InitQueryable(predicate);

            var parsingResult = queryable.Parse(ctx, ignoreQueryFilters);

            var tableName = ctx.GetTableName(parsingResult);

            sqlBuilder.DELETE().FROM(tableName);

            if (!parsingResult.PredicateSql.IsNullOrWhiteSpace())
            {
                if (!parsingResult.FullSql.Contains("join", StringComparison.OrdinalIgnoreCase))
                {
                    sqlBuilder.WHERE(parsingResult.PredicateSql);
                }
                else //like DeleteRangeAsync<Comment>(c => c.Article.Id == id);
                {
                    var aliasSeparator = parsingResult.QuerySqlGenerator.LocalAliasSeparator;
                    sqlBuilder.WHERE(SqlRawHelper.ToSubQuerySqlForWhereSegment(queryable, ctx, aliasSeparator));
                }
            }

            parameters = parsingResult.Parameters;

            return sqlBuilder.ToString();
        }

        private static IQueryable<TEntity> InitQueryable<TEntity>(this IQueryable<TEntity> queryable, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class, IEntity
        {
            if (predicate is null)
                queryable = queryable.Where(e => 1 == 1);
            else
                queryable = queryable.Where(predicate);

            return queryable;
        }

        private static string GetTableName(this DbContext ctx, ParsedSelectResult parsingResult)
        {
            return ctx.GetService<ISqlGenerationHelper>().DelimitIdentifier(parsingResult.TableName, parsingResult.Schema);
        }
    }
}