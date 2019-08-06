using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Pomelo.EntityFrameworkCore.Lolita.Delete
{
    /// <summary>
    /// Interface for lolita delete executor
    /// </summary>
    public interface ILolitaDeleteExecutor
    {
        /// <summary>
        /// Generate sql
        /// </summary>
        /// <param name="lolita"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        string GenerateSql<TEntity>(IQueryable<TEntity> lolita) where TEntity : class, new();

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        int Execute(DbContext db, string sql);

        /// <summary>
        /// Execute async
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> ExecuteAsync(DbContext db, string sql, CancellationToken cancellationToken = default(CancellationToken));
    }
}