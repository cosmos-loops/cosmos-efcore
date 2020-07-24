using System;
using System.Threading;
using System.Threading.Tasks;
using Cosmos.Data.Common;

namespace Cosmos.EntityFrameworkCore
{
    /// <summary>
    /// Interface for DbContext
    /// </summary>
    public interface IEfContext : IStoreContext, IDisposable
    {
        #region Save changes

        /// <summary>
        /// Save change
        /// </summary>
        /// <returns></returns>
        int SaveChanges();

        /// <summary>
        /// Save change async
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        #endregion
    }
}