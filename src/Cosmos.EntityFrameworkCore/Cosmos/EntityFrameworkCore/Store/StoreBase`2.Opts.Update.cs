using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cosmos.Domain.EntityDescriptors;
using Microsoft.EntityFrameworkCore;

namespace Cosmos.EntityFrameworkCore.Store
{
    public abstract partial class StoreBase<TContext, TEntity, TKey>
    {
        /// <summary>
        /// Update a entity
        /// </summary>
        /// <param name="entity"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual void Update(TEntity entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            RawTypedContext.Entry(entity).State = EntityState.Detached;

            var origin = FindById(entity.Id);
            var originEntry = RawTypedContext.Entry(origin);

            if (entity is IVersionable version)
            {
                originEntry.State = EntityState.Detached;
                originEntry.CurrentValues[nameof(version.Version)] = version.Version;
                originEntry = RawTypedContext.Attach(origin);
            }

            originEntry.CurrentValues.SetValues(entity);
        }

        /// <summary>
        /// Add a set of entity
        /// </summary>
        /// <param name="entities"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual void Update(IEnumerable<TEntity> entities)
        {
            if (entities is null)
                throw new ArgumentNullException(nameof(entities));
            foreach (var entity in entities)
                Update(entity);
        }

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual Task UpdateAsync(TEntity entity)
        {
            Update(entity);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Update a set of entity
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task UpdateAsync(IEnumerable<TEntity> entities)
        {
            if (entities is null)
                throw new ArgumentNullException(nameof(entities));
            foreach (var entity in entities)
                await UpdateAsync(entity);
        }
    }
}