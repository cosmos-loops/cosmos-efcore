using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Cosmos.Collections;
using Cosmos.Domain.EntityDescriptors;

namespace Cosmos.EntityFrameworkCore.Store
{
    public abstract partial class StoreBase<TContext, TEntity, TKey>
    {
        #region Remove # in base IStore.WriteableStore

        /// <summary>
        /// Remove a entity by id
        /// </summary>
        /// <param name="id"></param>
        public virtual void Remove(TKey id)
        {
            var entity = FindById(id);
            InternalDelete(entity, IncludeUnsafeOpt);
        }

        /// <summary>
        /// Remove a entity by entity
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Remove(TEntity entity)
        {
            if (entity is null)
                return;
            Remove(entity.Id);
        }

        /// <summary>
        /// Remove a set of entity by a set of given entity
        /// </summary>
        /// <param name="entities"></param>
        public virtual void Remove(IEnumerable<TEntity> entities)
        {
            if (entities is null)
                return;
            var entities2 = FindByIds(entities.Select(x => x.Id));
            InternalDelete(entities2, IncludeUnsafeOpt);
        }

        /// <summary>
        /// Remove by given condition
        /// </summary>
        /// <param name="predicate"></param>
        public virtual void Remove(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate is null)
                return;
            var entities = Find(predicate);
            InternalDelete(entities, IncludeUnsafeOpt);
        }

        /// <summary>
        /// Remove a entity by given id async
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task RemoveAsync(TKey id, CancellationToken cancellationToken = default)
        {
            var entity = await FindByIdAsync(id, cancellationToken);
            InternalDelete(entity, IncludeUnsafeOpt);
        }

        /// <summary>
        /// Remove a entity by given entity async
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity is null)
                return;
            await RemoveAsync(entity.Id, cancellationToken);
        }

        /// <summary>
        /// Remove a set of given entity async
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task RemoveAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            if (entities is null)
                return;
            var entities2 = await FindByIdsAsync(entities.Select(x => x.Id), cancellationToken);
            InternalDelete(entities2, IncludeUnsafeOpt);
        }

        /// <summary>
        /// Remove by given condition async
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task RemoveAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            if (predicate is null)
                return;
            var entities = await FindAsync(predicate, cancellationToken);
            InternalDelete(entities, IncludeUnsafeOpt);
        }

        #endregion

        #region Remove # in IStore.WriteableStore

        /// <summary>
        /// Remove a set of entity by a set if id
        /// </summary>
        /// <param name="ids"></param>
        public virtual void Remove(IEnumerable<TKey> ids)
        {
            if (ids is null)
                return;
            var entities = FindByIds(ids);
            InternalDelete(entities, IncludeUnsafeOpt);
        }

        /// <summary>
        /// Remove a set of entity by a set of given id async
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task RemoveAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
        {
            if (ids is null)
                return;
            var entities = await FindByIdsAsync(ids, cancellationToken);
            InternalDelete(entities, IncludeUnsafeOpt);
        }

        #endregion

        private void InternalDelete(TEntity entity, bool includeUnsafeOpt = false)
        {
            if (entity is null)
                return;

            if (includeUnsafeOpt)
            {
                Set.Remove(entity);
                return;
            }

            // ReSharper disable once SuspiciousTypeConversion.Global
            if (entity is IDeletable model)
            {
                model.IsDeleted = true;
            }
        }

        private void InternalDelete(IEnumerable<TEntity> entities, bool includeUnsafeOpt = false)
        {
            if (entities is null)
                return;

            // ReSharper disable once PossibleMultipleEnumeration
            if (!entities.Any())
                return;

            if (includeUnsafeOpt)
            {
                // ReSharper disable once PossibleMultipleEnumeration
                Set.RemoveRange(entities);
                return;
            }

            // ReSharper disable once SuspiciousTypeConversion.Global
            if (entities is IEnumerable<IDeletable> models)
            {
                models.ForEach(model => model.IsDeleted = true);
            }
        }
    }
}