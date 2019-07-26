using Cosmos.Data.Store;
using Cosmos.Domain.Core;

namespace Cosmos.EntityFrameworkCore.Store
{
    public interface IRepository<TEntity, in TKey> : IStore<TEntity, TKey>, IUnsafeWriteableStore<TEntity, TKey>, IRepository
        where TEntity : class, IEntity<TKey>, new() { }
}