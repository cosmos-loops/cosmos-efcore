using Cosmos.Data.Store;
using Cosmos.Domain.Core;

namespace Cosmos.EntityFrameworkCore.Store
{
    public abstract class RepositoryBase<TContext, TEntity, TKey> : StoreBase<TEntity, TKey>, IRepository
        where TContext : DbContextBase, IDbContext
        where TEntity : class, IEntity<TKey>, new()
    {
        protected RepositoryBase(TContext context) : base(context)
        {
            RawTypedContext = context;
        }

        protected RepositoryBase(TContext context, bool includeUnsafeOpt) : base(context, includeUnsafeOpt)
        {
            RawTypedContext = context;
        }

        public virtual void ScopedInitialize(IStoreContextManager manager)
        {
            manager.Register(typeof(TContext), RawTypedContext);
        }

        protected TContext RawTypedContext { get; }
    }
}