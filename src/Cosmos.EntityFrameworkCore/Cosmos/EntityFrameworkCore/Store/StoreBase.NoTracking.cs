using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Cosmos.EntityFrameworkCore.Store
{
    public abstract partial class StoreBase<TEntity, TKey>
    {
        public virtual long Count(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate == null)
                return 0L;
            return NoTrackingSet.LongCount(predicate);
        }

        public virtual async Task<long> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate == null)
                return 0L;
            return await NoTrackingSet.LongCountAsync(predicate);
        }

        public virtual bool Exist(TKey id)
        {
            if (id.SafeString().IsNullOrWhiteSpace())
                return false;
            return Exist(t => Equals(id, t.Id));
        }

        public virtual bool Exist(TKey[] ids)
        {
            if (ids == null)
                return false;
            return Exist(t => ids.Contains(t.Id));
        }

        public virtual bool Exist(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate == null)
                return false;
            return NoTrackingSet.Any(predicate);
        }

        public virtual async Task<bool> ExistAsync(TKey id)
        {
            if (id.SafeString().IsNullOrWhiteSpace())
                return false;
            return await ExistAsync(t => Equals(id, t.Id));
        }

        public virtual async Task<bool> ExistAsync(TKey[] ids)
        {
            if (ids == null)
                return false;
            return await ExistAsync(t => ids.Contains(t.Id));
        }

        public virtual async Task<bool> ExistAsync(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate == null)
                return false;
            return await NoTrackingSet.AnyAsync(predicate);
        }
    }
}