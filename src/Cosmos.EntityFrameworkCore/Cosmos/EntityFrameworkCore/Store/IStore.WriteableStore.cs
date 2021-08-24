using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Cosmos.Models;
using Cosmos.Validation.Annotations;

namespace Cosmos.EntityFrameworkCore.Store
{
    /// <summary>
    /// Interface of Writeable store
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface IWriteableStoreAppend<TEntity, in TKey> where TEntity : class, IEntity<TKey>, new()
    {
        #region UpdateWith/UpdateWithout

        /// <summary>
        /// Update a entity with property names.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="propNames"></param>
        /// <exception cref="ArgumentNullException"></exception>
        void UpdateWith(TEntity entity, params string[] propNames);

        /// <summary>
        /// Update a entity with property names.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="propertyExpressions"></param>
        /// <exception cref="ArgumentNullException"></exception>
        void UpdateWith(TEntity entity, params Expression<Func<TEntity, object>>[] propertyExpressions);

        /// <summary>
        /// Update a entity without property names.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="propNames"></param>
        /// <exception cref="ArgumentNullException"></exception>
        void UpdateWithout(TEntity entity, params string[] propNames);

        /// <summary>
        /// Update a entity without property names.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="propertyExpressions"></param>
        /// <exception cref="ArgumentNullException"></exception>
        void UpdateWithout(TEntity entity, params Expression<Func<TEntity, object>>[] propertyExpressions);

        /// <summary>
        /// Update a entity with property names.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="propNames"></param>
        /// <exception cref="ArgumentNullException"></exception>
        Task UpdateWithAsync(TEntity entity, params string[] propNames);

        /// <summary>
        /// Update a entity with property names.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="propertyExpressions"></param>
        /// <exception cref="ArgumentNullException"></exception>
        Task UpdateWithAsync(TEntity entity, params Expression<Func<TEntity, object>>[] propertyExpressions);

        /// <summary>
        /// Update a entity without property names.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="propNames"></param>
        /// <exception cref="ArgumentNullException"></exception>
        Task UpdateWithoutAsync(TEntity entity, params string[] propNames);

        /// <summary>
        /// Update a entity without property names.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="propertyExpressions"></param>
        /// <exception cref="ArgumentNullException"></exception>
        Task UpdateWithoutAsync(TEntity entity, params Expression<Func<TEntity, object>>[] propertyExpressions);

        #endregion

        #region Update Unsafe

        FluentUpdateBuilder<TEntity> UnsafeUpdate();

        #endregion

        #region Remove

        /// <summary>
        /// Remove by a set of id
        /// </summary>
        /// <param name="ids"></param>
        void Remove([NotNull] IEnumerable<TKey> ids);

        /// <summary>
        /// Remove by a set of id async
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task RemoveAsync([NotNull] IEnumerable<TKey> ids, CancellationToken cancellationToken = default);

        #endregion

        #region Remove Unsafe

        /// <summary>
        /// Unsafe remove by a set of id
        /// </summary>
        /// <param name="ids"></param>
        void UnsafeRemove([NotNull] IEnumerable<TKey> ids);

        /// <summary>
        /// Unsafe remove range
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="ignoreQueryFilters"></param>
        /// <returns></returns>
        int UnsafeRemove(Expression<Func<TEntity, bool>> predicate = null, bool ignoreQueryFilters = false);

        /// <summary>
        /// Unsafe remove by a set of id async
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task UnsafeRemoveAsync([NotNull] IEnumerable<TKey> ids, CancellationToken cancellationToken = default);

        /// <summary>
        /// Unsafe remove range async
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="ignoreQueryFilters"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> UnsafeRemoveAsync(Expression<Func<TEntity, bool>> predicate = null, bool ignoreQueryFilters = false, CancellationToken cancellationToken = default);

        #endregion
    }
}