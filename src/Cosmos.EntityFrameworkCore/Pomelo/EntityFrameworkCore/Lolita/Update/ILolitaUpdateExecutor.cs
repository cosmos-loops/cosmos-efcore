using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Pomelo.EntityFrameworkCore.Lolita.Update
{
    /// <summary>
    /// Interface for Lolita Update executor
    /// </summary>
    public interface ILolitaUpdateExecutor
    {
        /// <summary>
        /// Generate sql
        /// </summary>
        /// <param name="lolita"></param>
        /// <param name="visitor"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        string GenerateSql<TEntity>(LolitaSetting<TEntity> lolita, RelationalQueryModelVisitor visitor) where TEntity : class, new();

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        int Execute(DbContext db, string sql, object[] param);

        /// <summary>
        /// Execute async
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<int> ExecuteAsync(DbContext db, string sql, CancellationToken cancellationToken = default(CancellationToken), params object[] param);
    }
}