using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Cosmos.Domain.EntityDescriptors;
using Microsoft.EntityFrameworkCore;

namespace Cosmos.EntityFrameworkCore.Store
{
    public abstract partial class StoreBase<TEntity, TKey>
    {
        /// <summary>
        /// Add entity
        /// </summary>
        /// <param name="entity"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual void Add(TEntity entity)
        {
            if (entity == null)
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
            if (entities == null)
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
            if (entity == null)
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
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));
            await Set.AddRangeAsync(entities, cancellationToken);
        }

        /// <summary>
        /// Update a entity
        /// </summary>
        /// <param name="entity"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual void Update(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            Context.Entry(entity).State = EntityState.Detached;

            var origin = FindById(entity.Id);
            var originEntry = Context.Entry(origin);

            if (entity is IVersionable version)
            {
                originEntry.State = EntityState.Detached;
                originEntry.CurrentValues[nameof(version.Version)] = version.Version;
                originEntry = Context.Attach(origin);
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
            if (entities == null)
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
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));
            foreach (var entity in entities)
                await UpdateAsync(entity);
        }

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
            if (entity == null)
                return;
            Remove(entity.Id);
        }

        /// <summary>
        /// Remove a set of entity by a set if id
        /// </summary>
        /// <param name="ids"></param>
        public virtual void Remove(IEnumerable<TKey> ids)
        {
            if (ids == null)
                return;
            var entities = FindByIds(ids);
            Remove(entities);
        }

        /// <summary>
        /// Remove a set of entity by a set of given entity
        /// </summary>
        /// <param name="entities"></param>
        public virtual void Remove(IEnumerable<TEntity> entities)
        {
            if (entities == null)
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
            if (predicate == null)
                return;
            var entities = Find(predicate).ToList();
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
            if (entity == null)
                return;
            await RemoveAsync(entity.Id, cancellationToken);
        }

        /// <summary>
        /// Remove a set of entity by a set of given id async
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task RemoveAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
        {
            if (ids == null)
                return;
            var entities = await FindByIdsAsync(ids, cancellationToken);
            InternalDelete(entities, IncludeUnsafeOpt);
        }

        /// <summary>
        /// Remove a set of given entity async
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task RemoveAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            if (entities == null)
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
            if (predicate == null)
                return;
            var entities = await Find(predicate).ToListAsync(cancellationToken);
            InternalDelete(entities, IncludeUnsafeOpt);
        }

        private void InternalDelete(TEntity entity, bool includeUnsafeOpt = false)
        {
            if (entity == null)
                return;
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (entity is IDeletable model)
                model.IsDeleted = true;
            else if (includeUnsafeOpt)
                Set.Remove(entity);
        }

        private void InternalDelete(List<TEntity> entities, bool includeUnsafeOpt = false)
        {
            if (entities == null)
                return;
            if (!entities.Any())
                return;
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (entities[0] is IDeletable)
            {
                // ReSharper disable once SuspiciousTypeConversion.Global
                foreach (var entity in entities.Select(t => (IDeletable) t))
                    entity.IsDeleted = true;
            }
            else if (includeUnsafeOpt)
            {
                Set.RemoveRange(entities);
            }
        }
    }
}