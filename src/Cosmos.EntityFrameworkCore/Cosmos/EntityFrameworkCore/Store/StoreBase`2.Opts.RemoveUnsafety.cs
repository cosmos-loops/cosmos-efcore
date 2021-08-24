using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Cosmos.EntityFrameworkCore.SqlRaw;

namespace Cosmos.EntityFrameworkCore.Store
{
    public abstract partial class StoreBase<TContext, TEntity, TKey>
    {
        #region Remove unsafe # in base IStore.WriteableStore

        /// <summary>
        /// Unsafe remove
        /// </summary>
        /// <param name="id"></param>
        public virtual void UnsafeRemove(TKey id)
        {
            var entity = FindById(id);
            InternalDelete(entity, true);
        }

        /// <summary>
        /// Unsafe remove
        /// </summary>
        /// <param name="entity"></param>
        public virtual void UnsafeRemove(TEntity entity)
        {
            if (entity is null)
                return;
            UnsafeRemove(entity.Id);
        }

        /// <summary>
        /// Unsafe remove
        /// </summary>
        /// <param name="entities"></param>
        public virtual void UnsafeRemove(IEnumerable<TEntity> entities)
        {
            if (entities is null)
                return;
            var retouchedEntities = FindByIds(entities.Select(x => x.Id));
            InternalDelete(retouchedEntities, true);
        }

        /// <summary>
        /// Unsafe remove async
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task UnsafeRemoveAsync(TKey id, CancellationToken cancellationToken = default)
        {
            var entity = await FindByIdAsync(id, cancellationToken);
            InternalDelete(entity, true);
        }

        /// <summary>
        /// Unsafe remove async
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task UnsafeRemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity is null)
                return;
            await UnsafeRemoveAsync(entity.Id, cancellationToken);
        }

        /// <summary>
        /// Unsafe remove async
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task UnsafeRemoveAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            if (entities is null)
                return;
            var retouchedEntities = await FindByIdsAsync(entities.Select(x => x.Id), cancellationToken);
            InternalDelete(retouchedEntities, true);
        }

        #endregion

        #region Remove unsafe # in IStore.WriteableStore

        /// <summary>
        /// Unsafe remove
        /// </summary>
        /// <param name="ids"></param>
        public virtual void UnsafeRemove(IEnumerable<TKey> ids)
        {
            if (ids is null)
                return;
            var entities = FindByIds(ids);
            InternalDelete(entities, true);
        }

        /// <summary>
        /// Unsafe remove range
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="ignoreQueryFilters"></param>
        /// <returns></returns>
        public virtual int UnsafeRemove(Expression<Func<TEntity, bool>> predicate = null, bool ignoreQueryFilters = false)
        {
            var sql = SqlRawDeleteCommandGenerator.Generate(RawTypedContext, Set, predicate, ignoreQueryFilters, out var parameters);
            return SqlRawWorker.Execute(RawTypedContext, sql, parameters);
        }

        /// <summary>
        /// Unsafe remove async
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task UnsafeRemoveAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
        {
            if (ids is null)
                return;
            var entities = await FindByIdsAsync(ids, cancellationToken);
            InternalDelete(entities, true);
        }

        /// <summary>
        /// Unsafe remove range async
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="ignoreQueryFilters"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<int> UnsafeRemoveAsync(Expression<Func<TEntity, bool>> predicate = null, bool ignoreQueryFilters = false, CancellationToken cancellationToken = default)
        {
            var sql = SqlRawDeleteCommandGenerator.Generate(RawTypedContext, Set, predicate, ignoreQueryFilters, out var parameters);
            return await SqlRawWorker.ExecuteAsync(RawTypedContext, sql, parameters, cancellationToken);
        }

        #endregion
    }
}