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
    internal static class DatabaseExtesions
    {
        public static EntityQueryModelVisitor CreateVisitor(this Database self, QueryModel qm)
        {
            var databaseTypeInfo = typeof(Database).GetTypeInfo();
            var _queryCompilationContextFactory = (IQueryCompilationContextFactory)databaseTypeInfo.DeclaredFields.Single(x => x.Name == "_queryCompilationContextFactory").GetValue(self);
            return _queryCompilationContextFactory.Create(async: false).CreateQueryModelVisitor();
        }
    }
}