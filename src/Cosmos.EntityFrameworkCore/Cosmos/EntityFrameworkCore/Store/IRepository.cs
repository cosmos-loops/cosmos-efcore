using Cosmos.Data.Common;
using Cosmos.Data.Store;
using Cosmos.Domain.Core;

namespace Cosmos.EntityFrameworkCore.Store
{
    /// <summary>
    /// Interface for reposiory
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface IRepository<TEntity, in TKey> : IStore<TEntity, TKey>, IUnsafeWriteableStore<TEntity, TKey>, IRepository
        where TEntity : class, IEntity<TKey>, new() { }
}