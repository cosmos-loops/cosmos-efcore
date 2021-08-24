using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Linq;
using System.Linq.Expressions;
using Cosmos.EntityFrameworkCore.Internals;

/*
 * Reference: Zack.EFCore.Batch
 *   url: https://github.com/yangzhongke/Zack.EFCore.Batch
 *   author: 杨中科
 */

namespace Cosmos.EntityFrameworkCore.SqlRaw
{
    internal static class SqlRawGeneratorUtils
    {
        public static Expression VisitSelect(ISqlRawQueryGenerator sqlGenerator, ISqlGenerationHelper sqlGenerationHelper, SelectExpression selectExpression)
        {
            if (selectExpression.IsNonComposedSetOperation())
            {
                sqlGenerator.LocalGenerateSetOperation((SetOperationBase)selectExpression.Tables[0]);
                return selectExpression;
            }

            var sql = sqlGenerator.LocalSql;

            IDisposable disposable = null;

            if (selectExpression.Alias is not null)
            {
                sql.AppendLine("(");
                disposable = sqlGenerator.LocalSql.Indent();
            }

            sql.SELECT();

            if (selectExpression.IsDistinct)
                sql.DISTINCT();

            sqlGenerator.LocalGenerateTop(selectExpression);

            if (selectExpression.Projection.Any())
            {
                SqlRawHelper.GenerateList(selectExpression.Projection, sql,
                    delegate(ProjectionExpression exp)
                    {
                        var oldSql = sql.Build().CommandText;
                        var column = SqlRawHelper.DiffSql(oldSql, sql.Build().CommandText);
                        sqlGenerator.Visit(exp);
                        sqlGenerator.ProjectionSql.Add(column);
                    }
                );
            }
            else
            {
                sql.Append("1");
                sqlGenerator.ProjectionSql.Add("1");
            }

            if (selectExpression.Tables.Any())
                sql.NEWLINE().FROM(sqlGenerator, selectExpression);
            else
                sqlGenerator.LocalGeneratePseudoFromClause();

            if (selectExpression.Predicate is not null)
                sql.NEWLINE().WHERE(sqlGenerator, selectExpression);

            if (selectExpression.GroupBy.Count > 0)
                sql.NEWLINE().GROUPBY(sqlGenerator, selectExpression);

            if (selectExpression.Having is not null)
                sql.NEWLINE().HAVING(sqlGenerator, selectExpression);

            sqlGenerator.LocalGenerateOrderings(selectExpression);
            sqlGenerator.LocalGenerateLimitOffset(selectExpression);

            if (selectExpression.Alias is not null)
            {
                disposable?.Dispose();
                sql.NEWLINE().Append(")" + sqlGenerator.LocalAliasSeparator + sqlGenerationHelper.DelimitIdentifier(selectExpression.Alias));
            }

            return selectExpression;
        }
    }
}