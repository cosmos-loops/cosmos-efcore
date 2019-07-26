using System;
using System.Threading;
using System.Threading.Tasks;
using Cosmos.Data.Store;
using IRootDbContext = Cosmos.Data.Context.IDbContext;

namespace Cosmos.EntityFrameworkCore
{
    public interface IDbContext : IRootDbContext, IStoreContext, IDisposable
    {
        #region Save changes

        int SaveChanges();
        
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        #endregion
    }
}