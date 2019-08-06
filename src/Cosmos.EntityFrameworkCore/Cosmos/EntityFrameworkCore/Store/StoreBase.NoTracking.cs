using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Cosmos.EntityFrameworkCore.Store
{
    public abstract partial class StoreBase<TEntity, TKey>
    {
        /// <summary>
        /// Count
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual long Count(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate == null)
                return 0L;
            return NoTrackingSet.LongCount(predicate);
        }

        /// <summary>
        /// Count async
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual async Task<long> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate == null)
                return 0L;
            return await NoTrackingSet.LongCountAsync(predicate);
        }

        /// <summary>
        /// Exist
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool Exist(TKey id)
        {
            if (id.SafeString().IsNullOrWhiteSpace())
                return false;
            return Exist(t => Equals(id, t.Id));
        }

        /// <summary>
        /// Exist
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual bool Exist(TKey[] ids)
        {
            if (ids == null)
                return false;
            return Exist(t => ids.Contains(t.Id));
        }

        /// <summary>
        /// Exist
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual bool Exist(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate == null)
                return false;
            return NoTrackingSet.Any(predicate);
        }

        /// <summary>
        /// Exist async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<bool> ExistAsync(TKey id)
        {
            if (id.SafeString().IsNullOrWhiteSpace())
                return false;
            return await ExistAsync(t => Equals(id, t.Id));
        }

        /// <summary>
        /// Exist async
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual async Task<bool> ExistAsync(TKey[] ids)
        {
            if (ids == null)
                return false;
            return await ExistAsync(t => ids.Contains(t.Id));
        }

        /// <summary>
        /// Exist async
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual async Task<bool> ExistAsync(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate == null)
                return false;
            return await NoTrackingSet.AnyAsync(predicate);
        }
    }
}