using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Cosmos.Models;
using Cosmos.Validation.Annotations;

namespace Cosmos.EntityFrameworkCore.Store
{
    /// <summary>
    /// Interface of Queryable store
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface IQueryableStoreAppend<TEntity, in TKey> where TEntity : class, IEntity<TKey>, new()
    {
        #region Exist by id

        /// <summary>
        /// Exists or not by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Exist(TKey id);

        /// <summary>
        /// Exists or not by a set of id
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        bool Exist(TKey[] ids);

        /// <summary>
        /// Exists or not by a set of id
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        bool Exist([NotNull] IEnumerable<TKey> ids);

        /// <summary>
        /// Exists or not by id async
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> ExistAsync(TKey id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Exists or not by a set of id async
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> ExistAsync(TKey[] ids, CancellationToken cancellationToken = default);

        /// <summary>
        /// Exists or not by a set of id async
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> ExistAsync([NotNull] IEnumerable<TKey> ids, CancellationToken cancellationToken = default);

        #endregion

        #region Find by ids

        /// <summary>
        /// Find a collection of result by a set of id
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        IEnumerable<TEntity> FindByIds(params TKey[] ids);

        /// <summary>
        /// Find a collection of result by a set of id
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        IEnumerable<TEntity> FindByIds([NotNull] IEnumerable<TKey> ids);

        /// <summary>
        /// Find a collection of result by a set of id async
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> FindByIdsAsync(params TKey[] ids);

        /// <summary>
        /// Find a collection of result by a set of id async
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> FindByIdsAsync([NotNull] IEnumerable<TKey> ids, CancellationToken cancellationToken = default);

        #endregion

        #region Query

        /// <summary>
        /// Find a collection of result by by given condition
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IQueryable<TEntity> Query([NotNull] Expression<Func<TEntity, bool>> predicate);

        #endregion
    }
}