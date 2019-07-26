using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Pomelo.EntityFrameworkCore.Lolita;
using Pomelo.EntityFrameworkCore.Lolita.Update;

/*
 * reference to:
 *     PomeloFoundation/Lolita
 *     Author: Yuko & PomeloFoundation
 *     URL: https://github.com/PomeloFoundation/Lolita
 *     MIT
 */

namespace Microsoft.EntityFrameworkCore
{
    public static class UpdateExtensions
    {
        public static string GenerateBulkUpdateSql<TEntity>(this LolitaSetting<TEntity> self)
            where TEntity : class, new()
        {
            var modelVisitor = self.Query.CompileQuery();
            var executor = self.Query.GetService<ILolitaUpdateExecutor>();
            var sql = executor.GenerateSql(self, modelVisitor);
            self.GetService<ILoggerFactory>().CreateLogger("Lolita Bulk Updating").LogInformation(sql);
            return sql;
        }

        public static int Update<TEntity>(this LolitaSetting<TEntity> self)
            where TEntity : class, new()
        {
            var executor = self.Query.GetService<ILolitaUpdateExecutor>();
            var context = self.Query.GetService<ICurrentDbContext>().Context;
            return executor.Execute(context, self.GenerateBulkUpdateSql(), self.Parameters.ToArray());
        }

        public static Task<int> UpdateAsync<TEntity>(this LolitaSetting<TEntity> self, CancellationToken cancellationToken = default(CancellationToken))
            where TEntity : class, new()
        {
            var executor = self.Query.GetService<ILolitaUpdateExecutor>();
            var context = self.Query.GetService<ICurrentDbContext>().Context;
            return executor.ExecuteAsync(context, self.GenerateBulkUpdateSql(), cancellationToken, self.Parameters.ToArray());
        }
    }
}