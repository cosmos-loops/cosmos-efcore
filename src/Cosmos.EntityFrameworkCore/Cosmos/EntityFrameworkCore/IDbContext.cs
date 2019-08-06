using System;
using System.Threading;
using System.Threading.Tasks;
using Cosmos.Data.Store;
using IRootDbContext = Cosmos.Data.Context.IDbContext;

namespace Cosmos.EntityFrameworkCore
{
    /// <summary>
    /// Interface for DbContext
    /// </summary>
    public interface IDbContext : IRootDbContext, IStoreContext, IDisposable
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