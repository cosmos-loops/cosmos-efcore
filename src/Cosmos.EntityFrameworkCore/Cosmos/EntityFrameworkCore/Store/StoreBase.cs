using System;
using System.Linq;
using Cosmos.Data.Store;
using Cosmos.Disposables;
using Cosmos.Domain.Core;
using Microsoft.EntityFrameworkCore;

namespace Cosmos.EntityFrameworkCore.Store
{
    public abstract partial class StoreBase<TEntity, TKey> :
        DisposableObjects, IStore<TEntity, TKey>, IUnsafeWriteableStore<TEntity, TKey>
        where TEntity : class, IEntity<TKey>, new()
    {
        protected StoreBase(DbContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        protected StoreBase(DbContext context, bool includeUnsafeOpt)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            IncludeUnsafeOpt = includeUnsafeOpt;
        }

        protected bool IncludeUnsafeOpt { get; }

        protected DbContext Context { get; }

        protected DbSet<TEntity> Set => Context.Set<TEntity>();

        protected IQueryable<TEntity> NoTrackingSet => Context.Set<TEntity>().AsNoTracking();
    }
}