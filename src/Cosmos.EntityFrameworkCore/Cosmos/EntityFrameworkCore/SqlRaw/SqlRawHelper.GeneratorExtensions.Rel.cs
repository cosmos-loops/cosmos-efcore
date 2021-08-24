using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

// ReSharper disable InconsistentNaming

namespace Cosmos.EntityFrameworkCore.SqlRaw
{
    internal static partial class SqlGeneratorExtensions
    {
        public static IRelationalCommandBuilder SELECT(this IRelationalCommandBuilder sqlBuilder)
        {
            sqlBuilder.Append("SELECT ");
            return sqlBuilder;
        }

        public static IRelationalCommandBuilder DISTINCT(this IRelationalCommandBuilder sqlBuilder)
        {
            sqlBuilder.Append("DISTINCT ");
            return sqlBuilder;
        }

        public static IRelationalCommandBuilder FROM(this IRelationalCommandBuilder sqlBuilder,
            ISqlRawQueryGenerator sqlGenerator, SelectExpression selectExpression)
        {
            sqlBuilder.Append("FROM ");
            SqlRawHelper.GenerateList(selectExpression.Tables, sqlBuilder,
                delegate(TableExpressionBase exp) { sqlGenerator.Visit(exp); },
                delegate(IRelationalCommandBuilder localSql) { localSql.AppendLine(); });
            return sqlBuilder;
        }

        public static IRelationalCommandBuilder FROM(this IRelationalCommandBuilder sqlBuilder, string tableName)
        {
            sqlBuilder.Append($"FROM {tableName} ");
            return sqlBuilder;
        }

        public static IRelationalCommandBuilder WHERE(this IRelationalCommandBuilder sqlBuilder,
            ISqlRawQueryGenerator sqlGenerator, SelectExpression selectExpression)
        {
            sqlBuilder.Append("WHERE ");
            var oldSql = sqlBuilder.Build().CommandText;
            sqlGenerator.Visit(selectExpression.Predicate);
            sqlGenerator.PredicateSql = SqlRawHelper.DiffSql(oldSql, sqlBuilder.Build().CommandText);
            return sqlBuilder;
        }

        public static IRelationalCommandBuilder NEWLINE(this IRelationalCommandBuilder sqlBuilder)
        {
            sqlBuilder.AppendLine();
            return sqlBuilder;
        }

        public static IRelationalCommandBuilder GROUPBY(this IRelationalCommandBuilder sqlBuilder,
            ISqlRawQueryGenerator sqlGenerator, SelectExpression selectExpression)
        {
            sqlBuilder.Append("GROUP BY ");
            SqlRawHelper.GenerateList(selectExpression.GroupBy, sqlBuilder,
                delegate(SqlExpression exp) { sqlGenerator.Visit(exp); });
            return sqlBuilder;
        }

        public static IRelationalCommandBuilder HAVING(this IRelationalCommandBuilder sqlBuilder,
            ISqlRawQueryGenerator sqlGenerator, SelectExpression selectExpression)
        {
            sqlBuilder.Append("HAVING ");
            sqlGenerator.Visit(selectExpression.Having);
            return sqlBuilder;
        }
    }
}