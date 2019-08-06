using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Cosmos.Domain.Core;
using Cosmos.Validations.Parameters;
using Pomelo.EntityFrameworkCore.Lolita;

namespace Cosmos.EntityFrameworkCore.Store
{
    /// <summary>
    /// Interface for Lolita writeable store
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface ILolitaWriteableStore<TEntity, in TKey> where TEntity : class, IEntity<TKey>, new()
    {
        /// <summary>
        /// Execute unsafe update
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="updateFunc"></param>
        /// <param name="updateFunc2"></param>
        void ExecuteUnsafeUpdate(
            [NotNull] Expression<Func<TEntity, bool>> predicate,
            [NotNull] Func<IQueryable<TEntity>, LolitaSetting<TEntity>> updateFunc,
            params Func<LolitaSetting<TEntity>, LolitaSetting<TEntity>>[] updateFunc2);

        /// <summary>
        /// Execute unsafe update async
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="updateFunc"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="updateFunc2"></param>
        /// <returns></returns>
        Task ExecuteUnsafeUpdateAsync(
            [NotNull] Expression<Func<TEntity, bool>> predicate,
            [NotNull] Func<IQueryable<TEntity>, LolitaSetting<TEntity>> updateFunc,
            CancellationToken cancellationToken = default,
            params Func<LolitaSetting<TEntity>, LolitaSetting<TEntity>>[] updateFunc2);

        /// <summary>
        /// Execute unsafe delete
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="predicate2"></param>
        void ExecuteUnsafeDelete(
            [NotNull] Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, bool>>[] predicate2);

        /// <summary>
        /// Execute unsafe delete async
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="predicate2"></param>
        /// <returns></returns>
        Task ExecuteUnsafeDeleteAsync(
            [NotNull] Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, bool>>[] predicate2);
    }
}