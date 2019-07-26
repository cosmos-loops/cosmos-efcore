using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cosmos.EntityFrameworkCore.Store
{
    public abstract partial class StoreBase<TEntity, TKey>
    {
        public virtual void UnsafeRemove(TKey id)
        {
            var entity = FindById(id);
            InternalDelete(entity, true);
        }

        public virtual void UnsafeRemove(TEntity entity)
        {
            if (entity == null)
                return;
            UnsafeRemove(entity.Id);
        }

        public virtual void UnsafeRemove(IEnumerable<TKey> ids)
        {
            if (ids == null)
                return;
            var entities = FindByIds(ids);
            UnsafeRemove(entities);
        }

        public virtual void UnsafeRemove(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                return;
            var entities2 = FindByIds(entities.Select(x => x.Id));
            InternalDelete(entities2, true);
        }

        public virtual async Task UnsafeRemoveAsync(TKey id, CancellationToken cancellationToken = default)
        {
            var entity = await FindByIdAsync(id, cancellationToken);
            InternalDelete(entity, true);
        }

        public virtual async Task UnsafeRemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                return;
            await UnsafeRemoveAsync(entity.Id, cancellationToken);
        }

        public virtual async Task UnsafeRemoveAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
        {
            if (ids == null)
                return;
            var entities = await FindByIdsAsync(ids, cancellationToken);
            InternalDelete(entities, true);
        }

        public virtual async Task UnsafeRemoveAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            if (entities == null)
                return;
            var entities2 = await FindByIdsAsync(entities.Select(x => x.Id), cancellationToken);
            InternalDelete(entities2, true);
        }
    }
}