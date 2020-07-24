using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Cosmos.Optionals;
using Cosmos.Text;
using Microsoft.EntityFrameworkCore;

namespace Cosmos.EntityFrameworkCore.Store
{
    public abstract partial class StoreBase<TContext, TEntity, TKey>
    {
        #region Exist # in base IStore.QueryableStore

        /// <summary>
        /// Exist
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual bool Exist(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate is null)
                return false;
            return NoTrackingSet.Any(predicate);
        }

        /// <summary>
        /// Exist async
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<bool> ExistAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            if (predicate is null)
                return false;
            return await NoTrackingSet.AnyAsync(predicate, cancellationToken);
        }

        #endregion

        #region Exist # in IStore.QueryableStore.Query

        /// <summary>
        /// Exist
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool Exist(TKey id)
        {
            if (id.SafeString().IsNullOrWhiteSpace())
                return false;
            return Exist(entity => Equals(id, entity.Id));
        }

        /// <summary>
        /// Exist
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual bool Exist(TKey[] ids)
        {
            if (ids is null)
                return false;
            return Exist(entity => ids.Contains(entity.Id));
        }

        /// <summary>
        /// Exists or not by a set of id
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual bool Exist(IEnumerable<TKey> ids)
        {
            if (ids is null)
                return false;
            return Exist(entity => ids.Contains(entity.Id));
        }

        /// <summary>
        /// Exist async
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<bool> ExistAsync(TKey id, CancellationToken cancellationToken = default)
        {
            if (id.SafeString().IsNullOrWhiteSpace())
                return false;
            return await ExistAsync(entity => Equals(id, entity.Id), cancellationToken);
        }

        /// <summary>
        /// Exist async
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<bool> ExistAsync(TKey[] ids, CancellationToken cancellationToken = default)
        {
            if (ids is null)
                return false;
            return await ExistAsync(entity => ids.Contains(entity.Id), cancellationToken);
        }

        /// <summary>
        /// Exists or not by a set of id async
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<bool> ExistAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
        {
            if (ids is null)
                return false;
            return await ExistAsync(entity => ids.Contains(entity.Id), cancellationToken);
        }

        #endregion
    }
}