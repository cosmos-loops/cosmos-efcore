using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.ToSql.Internals;
using Remotion.Linq;
using Remotion.Linq.Parsing.Structure;

/*
 * reference to:
 *     PomeloFoundation/Pomelo.EntityFrameworkCore.Extensions.ToSql
 *     Author: Yuko & PomeloFoundation
 *     URL: https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.Extensions.ToSql
 *     MIT
 */

namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Extensions for QueryToSql
    /// </summary>
    public static class QueryToSqlExtensions
    {
        /// <summary>
        /// To sql
        /// </summary>
        /// <param name="self"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static string ToSql<TEntity>(this IQueryable<TEntity> self)
        {
            var visitor = self.CompileQuery();
            return string.Join("", visitor.Queries.Select(x => x.ToString().TrimEnd().TrimEnd(';') + ";" + Environment.NewLine));
        }

        /// <summary>
        /// To Unevaluated
        /// </summary>
        /// <param name="self"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static IEnumerable<string> ToUnevaluated<TEntity>(this IQueryable<TEntity> self)
        {
            var visitor = self.CompileQuery();
            return QueryModelVisitorUtils.VisitExpression(visitor.Expression, null);
        }

        /// <summary>
        /// Compile query
        /// </summary>
        /// <param name="self"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static RelationalQueryModelVisitor CompileQuery<TEntity>(this IQueryable<TEntity> self)
        {
            var q = self as EntityQueryable<TEntity>;
            if (q == null)
            {
                return null;
            }

            var fields = typeof(Database).GetTypeInfo().DeclaredFields;

            var queryCompiler = (QueryCompiler) ReflectionCommon.QueryCompilerOfEntityQueryProvider.GetValue(self.Provider);
            var database = (Database) ReflectionCommon.DatabaseOfQueryCompiler.GetValue(queryCompiler);
            var dependencies = (DatabaseDependencies) ReflectionCommon.DependenciesOfDatabase.GetValue(database);
            var factory = dependencies.QueryCompilationContextFactory;
            var nodeTypeProvider = (INodeTypeProvider) ReflectionCommon.NodeTypeProvider.GetValue(queryCompiler);
            var parser = (QueryParser) ReflectionCommon.CreateQueryParserMethod.Invoke(queryCompiler, new object[] {nodeTypeProvider});
            var queryModel = parser.GetParsedQuery(self.Expression);
            var modelVisitor = (RelationalQueryModelVisitor) database.CreateVisitor(factory, queryModel);
            modelVisitor.CreateQueryExecutor<TEntity>(queryModel);
            return modelVisitor;
        }

        /// <summary>
        /// Create visitor
        /// </summary>
        /// <param name="self"></param>
        /// <param name="factory"></param>
        /// <param name="qm"></param>
        /// <returns></returns>
        public static EntityQueryModelVisitor CreateVisitor(this Database self, IQueryCompilationContextFactory factory, QueryModel qm)
        {
            return factory.Create(async: false).CreateQueryModelVisitor();
        }
    }
}