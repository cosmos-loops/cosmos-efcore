using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Cosmos.EntityFrameworkCore.Store
{
    public abstract partial class StoreBase<TContext, TEntity, TKey>
    {
        #region Count # in base IStore.QueryableStore

        /// <summary>
        /// Count
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual long Count(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate is null)
                return 0L;
            return NoTrackingSet.LongCount(predicate);
        }

        /// <summary>
        /// Count async
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<long> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            if (predicate is null)
                return 0L;
            return await NoTrackingSet.LongCountAsync(predicate, cancellationToken);
        }

        #endregion
    }
}