#pragma warning disable EF1001
using System;
using System.Linq;
using System.Linq.Expressions;
using Cosmos.EntityFrameworkCore.SqlRaw;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

/*
 * Reference: Zack.EFCore.Batch
 *   url: https://github.com/yangzhongke/Zack.EFCore.Batch
 *   author: 杨中科
 */

namespace Cosmos.EntityFrameworkCore.Internals
{
    internal static class SelectExpressionExtensions
    {
        /// <summary>
        /// parse select statement of queryable
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="ctx"></param>
        /// <param name="ignoreQueryFilters"></param>
        /// <returns></returns>
        public static ParsedSelectResult Parse<TEntity>(this IQueryable<TEntity> queryable, DbContext ctx, bool ignoreQueryFilters) where TEntity : class
        {
            if (ignoreQueryFilters)
                queryable = queryable.IgnoreQueryFilters();

            var query = queryable.Expression;
            var queryCompilationContext = ctx.GetService<DatabaseDependencies>().QueryCompilationContextFactory.Create(true);
            var logger = ctx.GetService<IDiagnosticsLogger<DbLoggerCategory.Query>>();

            var queryContext = ctx.GetService<IQueryContextFactory>().Create();

            if (ctx.GetService<IQueryCompiler>() is not QueryCompiler queryCompiler)
                throw new InvalidOperationException($"Cannot get {typeof(IQueryCompiler)} service.");

            //parameterize determines if it will use "Declare" or not
            var methodCallExpr1 = queryCompiler.ExtractParameters(query, queryContext, logger, parameterize: true); // as MethodCallExpression;
            var queryTranslationPreprocessor = ctx.GetService<IQueryTranslationPreprocessorFactory>().Create(queryCompilationContext);
            var methodCallExpr2 = queryTranslationPreprocessor.Process(methodCallExpr1) as MethodCallExpression;
            var queryableMethodTranslatingExpressionVisitor = ctx.GetService<IQueryableMethodTranslatingExpressionVisitorFactory>().Create(queryCompilationContext);
            var shapedQueryExpression1 = queryableMethodTranslatingExpressionVisitor.Visit(methodCallExpr2) as ShapedQueryExpression;
            var queryTranslationPostprocessor = ctx.GetService<IQueryTranslationPostprocessorFactory>().Create(queryCompilationContext);
            var shapedQueryExpression2 = queryTranslationPostprocessor.Process(shapedQueryExpression1!) as ShapedQueryExpression;

            var selectExpression = (SelectExpression)shapedQueryExpression2!.QueryExpression;

            selectExpression = ctx.GetService<IRelationalParameterBasedSqlProcessorFactory>()
                                  .Create(true)
                                  .Optimize(selectExpression, queryContext.ParameterValues, out _);

            if (ctx.GetService<IQuerySqlGeneratorFactory>().Create() is not ISqlRawQueryGenerator querySqlGenerator)
                throw new InvalidOperationException("Please add dbContext.UseRangeOperations() to OnConfiguring first.");

            querySqlGenerator.EnableRangeOperation = true;
            var relationalCommand = querySqlGenerator.GetCommand(selectExpression);
            var tableExpression = selectExpression.Tables[0] as TableExpression;

            var parsingResult = new ParsedSelectResult
            {
                Parameters = queryContext.ParameterValues,
                QuerySqlGenerator = querySqlGenerator,
                PredicateSql = querySqlGenerator.PredicateSql,
                ProjectionSql = querySqlGenerator.ProjectionSql,
                TableName = tableExpression!.Table.Name,
                Schema = tableExpression.Schema,
                FullSql = relationalCommand.CommandText
            };

            return parsingResult;
        }

        //this method is from source code ef core
        public static bool IsNonComposedSetOperation(this SelectExpression selectExpression)
        {
            if (selectExpression.Offset is null
             && selectExpression.Limit is null
             && !selectExpression.IsDistinct
             && selectExpression.Predicate is null
             && selectExpression.Having is null
             && selectExpression.Orderings.Count == 0
             && selectExpression.GroupBy.Count == 0
             && selectExpression.Tables.Count == 1)
            {
                var tableExpressionBase = selectExpression.Tables[0];
                if (tableExpressionBase is not SetOperationBase setOperation)
                    return false;
                if (selectExpression.Projection.Count == setOperation.Source1.Projection.Count)
                    return selectExpression.Projection.Select(delegate(ProjectionExpression pe, int index)
                    {
                        if (pe.Expression is ColumnExpression columnExpression
                         && string.Equals(columnExpression.Table.Alias, setOperation.Alias, StringComparison.OrdinalIgnoreCase))
                            return string.Equals(columnExpression.Name, setOperation.Source1.Projection[index].Alias, StringComparison.OrdinalIgnoreCase);
                        return false;
                    }).All(e => e);
            }

            return false;
        }
    }
}
#pragma warning restore EF1001