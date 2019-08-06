using Cosmos.Data.Store;
using Cosmos.Domain.Core;

namespace Cosmos.EntityFrameworkCore.Store
{
    /// <summary>
    /// Repository base
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public abstract class RepositoryBase<TContext, TEntity, TKey> : StoreBase<TEntity, TKey>, IRepository
        where TContext : DbContextBase, IDbContext
        where TEntity : class, IEntity<TKey>, new()
    {
        /// <summary>
        /// Repository base
        /// </summary>
        /// <param name="context"></param>
        protected RepositoryBase(TContext context) : base(context)
        {
            RawTypedContext = context;
        }

        /// <summary>
        /// Repository base
        /// </summary>
        /// <param name="context"></param>
        /// <param name="includeUnsafeOpt"></param>
        protected RepositoryBase(TContext context, bool includeUnsafeOpt) : base(context, includeUnsafeOpt)
        {
            RawTypedContext = context;
        }

        /// <summary>
        /// Scoped initialize
        /// </summary>
        /// <param name="manager"></param>
        public virtual void ScopedInitialize(IStoreContextManager manager)
        {
            manager.Register(typeof(TContext), RawTypedContext);
        }

        /// <summary>
        /// Raw typed context
        /// </summary>
        protected TContext RawTypedContext { get; }
    }
}