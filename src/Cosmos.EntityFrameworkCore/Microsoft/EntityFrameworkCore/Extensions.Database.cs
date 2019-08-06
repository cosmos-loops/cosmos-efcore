using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Remotion.Linq;

/*
 * reference to:
 *     PomeloFoundation/Lolita
 *     Author: Yuko & PomeloFoundation
 *     URL: https://github.com/PomeloFoundation/Lolita
 *     MIT
 */

namespace Microsoft.EntityFrameworkCore
{
    internal static class DatabaseExtensions
    {
        /// <summary>
        /// Create visitor
        /// </summary>
        /// <param name="self"></param>
        /// <param name="qm"></param>
        /// <returns></returns>
        public static EntityQueryModelVisitor CreateVisitor(this Database self, QueryModel qm)
        {
            var databaseTypeInfo = typeof(Database).GetTypeInfo();
            var queryCompilationContextFactory = (IQueryCompilationContextFactory)databaseTypeInfo.DeclaredFields.Single(x => x.Name == "_queryCompilationContextFactory").GetValue(self);
            return queryCompilationContextFactory.Create(async: false).CreateQueryModelVisitor();
        }
    }
}