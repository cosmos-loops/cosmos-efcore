using Cosmos.Data.Store;
using Cosmos.Domain.Core;

namespace Cosmos.EntityFrameworkCore.Store
{
    /// <summary>
    /// Interface for store
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface IStore<TEntity, in TKey> :
        IQueryableStore<TEntity, TKey>,
        IWriteableStore<TEntity, TKey>,
        ILolitaWriteableStore<TEntity, TKey>
        where TEntity : class, IEntity<TKey>, new() { }
}