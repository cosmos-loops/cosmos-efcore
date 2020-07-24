using Cosmos.Data.Common;
using Cosmos.Domain.Core;

namespace Cosmos.EntityFrameworkCore.Store
{
    /// <summary>
    /// Repository base
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public abstract class RepositoryBase<TContext, TEntity, TKey> : StoreBase<TContext, TEntity, TKey>, IRepository
        where TContext : DbContextBase, IEfContext, IDbContext
        where TEntity : class, IEntity<TKey>, new()
    {
        /// <summary>
        /// Repository base
        /// </summary>
        /// <param name="context"></param>
        protected RepositoryBase(TContext context) : base(context) { }

        /// <summary>
        /// Repository base
        /// </summary>
        /// <param name="context"></param>
        /// <param name="includeUnsafeOpt"></param>
        protected RepositoryBase(TContext context, bool includeUnsafeOpt) : base(context, includeUnsafeOpt) { }

        /// <inheritdoc />
        public string CurrentTraceId { get; set; }

        /// <inheritdoc />
        public IUnitOfWorkEntry UnitOfWork { get; set; }
    }
}