using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cosmos.Domain.Core;
using Cosmos.Validations.Parameters;

namespace Cosmos.EntityFrameworkCore.Store
{
    /// <summary>
    /// Interface of Writeable store
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface IWriteableStoreAppend<TEntity, in TKey> where TEntity : class, IEntity<TKey>, new()
    {
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
        /// Unsafe remove by a set of id async
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task UnsafeRemoveAsync([NotNull] IEnumerable<TKey> ids, CancellationToken cancellationToken = default);

        #endregion
    }
}