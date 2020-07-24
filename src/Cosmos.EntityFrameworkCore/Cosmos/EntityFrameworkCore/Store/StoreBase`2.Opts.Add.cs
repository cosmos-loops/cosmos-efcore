using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cosmos.EntityFrameworkCore.Store
{
    public abstract partial class StoreBase<TContext, TEntity, TKey>
    {
        #region Add # in base IStore.WriteableStore

        /// <summary>
        /// Add entity
        /// </summary>
        /// <param name="entity"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual void Add(TEntity entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));
            Set.Add(entity);
        }

        /// <summary>
        /// Add a set of entity
        /// </summary>
        /// <param name="entities"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual void Add(IEnumerable<TEntity> entities)
        {
            if (entities is null)
                throw new ArgumentNullException(nameof(entities));
            Set.AddRange(entities);
        }

        /// <summary>
        /// Add entity async
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));
            await Set.AddAsync(entity, cancellationToken);
        }

        /// <summary>
        /// Add a set of entity async
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task AddAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            if (entities is null)
                throw new ArgumentNullException(nameof(entities));
            await Set.AddRangeAsync(entities, cancellationToken);
        }

        #endregion
    }
}