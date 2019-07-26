using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;

/*
 * reference to:
 *     PomeloFoundation/Pomelo.EntityFrameworkCore.Extensions.ToSql
 *     Author: Yuko & PomeloFoundation
 *     URL: https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.Extensions.ToSql
 *     MIT
 */

namespace Pomelo.EntityFrameworkCore.ToSql.Internals
{
    internal static class ReflectionCommon
    {
        public static readonly FieldInfo QueryCompilerOfEntityQueryProvider = typeof(EntityQueryProvider).GetTypeInfo().DeclaredFields.First(x => x.Name == "_queryCompiler");
        public static readonly PropertyInfo DatabaseOfQueryCompiler = typeof(QueryCompiler).GetTypeInfo().DeclaredProperties.First(x => x.Name == "Database");
        public static readonly PropertyInfo DependenciesOfDatabase = typeof(Database).GetTypeInfo().DeclaredProperties.First(x => x.Name == "Dependencies");
        public static readonly TypeInfo QueryCompilerTypeInfo = typeof(QueryCompiler).GetTypeInfo();
        public static readonly MethodInfo CreateQueryParserMethod = QueryCompilerTypeInfo.DeclaredMethods.First(x => x.Name == "CreateQueryParser");
        public static readonly PropertyInfo NodeTypeProvider = QueryCompilerTypeInfo.DeclaredProperties.Single(x => x.Name == "NodeTypeProvider");
        public static readonly PropertyInfo QueriesOfRelationalQueryModelVisitor = typeof(RelationalQueryModelVisitor).GetTypeInfo().DeclaredProperties.Single(x => x.Name == "Queries");
    }
}