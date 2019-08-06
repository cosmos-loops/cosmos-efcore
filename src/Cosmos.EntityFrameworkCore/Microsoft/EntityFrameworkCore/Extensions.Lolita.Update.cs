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
    /// <summary>
    /// Extensions for update
    /// </summary>
    public static class UpdateExtensions
    {
        /// <summary>
        /// Generate bulk update sql
        /// </summary>
        /// <param name="self"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static string GenerateBulkUpdateSql<TEntity>(this LolitaSetting<TEntity> self)
            where TEntity : class, new()
        {
            var modelVisitor = self.Query.CompileQuery();
            var executor = self.Query.GetService<ILolitaUpdateExecutor>();
            var sql = executor.GenerateSql(self, modelVisitor);
            self.GetService<ILoggerFactory>().CreateLogger("Lolita Bulk Updating").LogInformation(sql);
            return sql;
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="self"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static int Update<TEntity>(this LolitaSetting<TEntity> self)
            where TEntity : class, new()
        {
            var executor = self.Query.GetService<ILolitaUpdateExecutor>();
            var context = self.Query.GetService<ICurrentDbContext>().Context;
            return executor.Execute(context, self.GenerateBulkUpdateSql(), self.Parameters.ToArray());
        }

        /// <summary>
        /// Update async
        /// </summary>
        /// <param name="self"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static Task<int> UpdateAsync<TEntity>(this LolitaSetting<TEntity> self, CancellationToken cancellationToken = default(CancellationToken))
            where TEntity : class, new()
        {
            var executor = self.Query.GetService<ILolitaUpdateExecutor>();
            var context = self.Query.GetService<ICurrentDbContext>().Context;
            return executor.ExecuteAsync(context, self.GenerateBulkUpdateSql(), cancellationToken, self.Parameters.ToArray());
        }
    }
}