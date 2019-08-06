using System;
using System.Linq;
using Cosmos.Data.Store;
using Cosmos.Disposables;
using Cosmos.Domain.Core;
using Microsoft.EntityFrameworkCore;

namespace Cosmos.EntityFrameworkCore.Store
{
    /// <summary>
    /// Store base
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public abstract partial class StoreBase<TEntity, TKey> :
        DisposableObjects, IStore<TEntity, TKey>, IUnsafeWriteableStore<TEntity, TKey>
        where TEntity : class, IEntity<TKey>, new()
    {
        /// <summary>
        /// Store base
        /// </summary>
        /// <param name="context"></param>
        protected StoreBase(DbContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Store base
        /// </summary>
        /// <param name="context"></param>
        /// <param name="includeUnsafeOpt"></param>
        protected StoreBase(DbContext context, bool includeUnsafeOpt)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            IncludeUnsafeOpt = includeUnsafeOpt;
        }

        /// <summary>
        /// Include unsafe opt
        /// </summary>
        protected bool IncludeUnsafeOpt { get; }

        /// <summary>
        /// Context
        /// </summary>
        protected DbContext Context { get; }

        /// <summary>
        /// Gets typed DbSet
        /// </summary>
        protected DbSet<TEntity> Set => Context.Set<TEntity>();

        /// <summary>
        /// No tracking set
        /// </summary>
        protected IQueryable<TEntity> NoTrackingSet => Context.Set<TEntity>().AsNoTracking();
    }
}