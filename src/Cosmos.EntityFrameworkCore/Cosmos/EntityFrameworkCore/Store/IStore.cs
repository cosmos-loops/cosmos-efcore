using Cosmos.Data.Store;
using Cosmos.Domain.Core;

namespace Cosmos.EntityFrameworkCore.Store
{
    public interface IStore<TEntity, in TKey> :
        IQueriableStore<TEntity, TKey>,
        IWriteableStore<TEntity, TKey>,
        ILolitaWriteableStore<TEntity, TKey>
        where TEntity : class, IEntity<TKey>, new() { }
}