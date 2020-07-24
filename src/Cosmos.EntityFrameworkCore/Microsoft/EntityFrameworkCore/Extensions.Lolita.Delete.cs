using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Pomelo.EntityFrameworkCore.Lolita.Delete;

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
    /// Extensions for delete
    /// </summary>
    public static class DeleteExtensions
    {
        /// <summary>
        /// Generate bulk delete sql
        /// </summary>
        /// <param name="self"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static string GenerateBulkDeleteSql<TEntity>(this IQueryable<TEntity> self)
        where TEntity : class, new()
        {
            var executor = self.GetService<ILolitaDeleteExecutor>();
            var sql = executor.GenerateSql(self);
            self.GetService<ILoggerFactory>().CreateLogger("Lolita Bulk Deleting").LogInformation(sql);
            return sql;
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="self"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static int Delete<TEntity>(this IQueryable<TEntity> self)
        where TEntity : class, new()
        {
            var executor = self.GetService<ILolitaDeleteExecutor>();
            var context = self.GetService<ICurrentDbContext>().Context;
            return executor.Execute(context, self.GenerateBulkDeleteSql());
        }

        /// <summary>
        /// Delete async
        /// </summary>
        /// <param name="self"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static Task<int> DeleteAsync<TEntity>(this IQueryable<TEntity> self, CancellationToken cancellationToken = default(CancellationToken))
        where TEntity : class, new()
        {
            var executor = self.GetService<ILolitaDeleteExecutor>();
            var context = self.GetService<ICurrentDbContext>().Context;
            return executor.ExecuteAsync(context, self.GenerateBulkDeleteSql(), cancellationToken);
        }
    }
}