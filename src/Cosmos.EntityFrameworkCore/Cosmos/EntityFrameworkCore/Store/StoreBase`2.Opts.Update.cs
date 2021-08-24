using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Cosmos.Collections;
using Cosmos.EntityFrameworkCore.Internals;
using Cosmos.Models.Descriptors.EntityDescriptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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
        /// Update a set of entity
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
        /// Update a entity with property names.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="propNames"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual void UpdateWith(TEntity entity, params string[] propNames)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            if (propNames is null || propNames.Length == 0)
            {
                Update(entity);
                return;
            }

            var entry = RawTypedContext.GetEntityEntry(entity, out var existBefore);

            if (existBefore)
            {
                foreach (var propEntry in entry.Properties)
                {
                    if (!propEntry.Metadata.Name.BeContainedIn(propNames))
                    {
                        propEntry.IsModified = false;
                    }
                }
            }
            else
            {
                entry.State = EntityState.Unchanged;
                foreach (var propName in propNames)
                {
                    entry.Property(propName).IsModified = true;
                }
            }
        }

        /// <summary>
        /// Update a entity with property names.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="propertyExpressions"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual void UpdateWith(TEntity entity, params Expression<Func<TEntity, object>>[] propertyExpressions)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            if (propertyExpressions is null || propertyExpressions.Length == 0)
            {
                Update(entity);
                return;
            }

            var entry = RawTypedContext.GetEntityEntry(entity, out var existBefore);

            if (existBefore)
            {
                var propNames = propertyExpressions.Select(x => x.GetMemberName()).ToArray();

                foreach (var propEntry in entry.Properties)
                {
                    if (!propEntry.Metadata.Name.BeContainedIn(propNames))
                    {
                        propEntry.IsModified = false;
                    }
                }
            }
            else
            {
                entry.State = EntityState.Unchanged;
                foreach (var expression in propertyExpressions)
                {
                    entry.Property(expression).IsModified = true;
                }
            }
        }

        /// <summary>
        /// Update a entity without property names.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="propNames"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual void UpdateWithout(TEntity entity, params string[] propNames)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            if (propNames is null || propNames.Length == 0)
            {
                Update(entity);
                return;
            }

            var entry = RawTypedContext.GetEntityEntry(entity, out _);
            entry.State = EntityState.Modified;
            foreach (var expression in propNames)
            {
                entry.Property(expression).IsModified = false;
            }
        }

        /// <summary>
        /// Update a entity without property names.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="propertyExpressions"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual void UpdateWithout(TEntity entity, params Expression<Func<TEntity, object>>[] propertyExpressions)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            if (propertyExpressions is null || propertyExpressions.Length == 0)
            {
                Update(entity);
                return;
            }

            var entry = RawTypedContext.GetEntityEntry(entity, out _);

            entry.State = EntityState.Modified;
            foreach (var expression in propertyExpressions)
            {
                entry.Property(expression).IsModified = false;
            }
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

        /// <summary>
        /// Update a entity with property names.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="propNames"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual Task UpdateWithAsync(TEntity entity, params string[] propNames)
        {
            UpdateWith(entity, propNames);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Update a entity with property names.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="propertyExpressions"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual Task UpdateWithAsync(TEntity entity, params Expression<Func<TEntity, object>>[] propertyExpressions)
        {
            UpdateWith(entity, propertyExpressions);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Update a entity without property names.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="propNames"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual  Task UpdateWithoutAsync(TEntity entity, params string[] propNames)
        {
            UpdateWithout(entity, propNames);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Update a entity without property names.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="propertyExpressions"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual Task UpdateWithoutAsync(TEntity entity, params Expression<Func<TEntity, object>>[] propertyExpressions)
        {
            UpdateWithout(entity, propertyExpressions);
            return Task.CompletedTask;
        }
    }
}