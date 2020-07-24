using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Cosmos.Data.Common;
using Cosmos.Data.Store;
using Cosmos.Disposables;
using Cosmos.Domain.Core;
using Cosmos.Domain.EntityDescriptors;
using Microsoft.EntityFrameworkCore;

namespace Cosmos.EntityFrameworkCore.Store
{
    /// <summary>
    /// Store base
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    public abstract partial class StoreBase<TContext, TEntity, TKey> :
        DisposableObjects, IStore<TEntity, TKey>, IUnsafeWriteableStore<TEntity, TKey>
        where TContext : DbContextBase, IEfContext, IDbContext
        where TEntity : class, IEntity<TKey>, new()
    {
        private readonly bool _includeUnsafeOpt;

        /// <summary>
        /// Store base
        /// </summary>
        /// <param name="context"></param>
        protected StoreBase(TContext context)
        {
            RawTypedContext = context ?? throw new ArgumentNullException(nameof(context));

            _includeUnsafeOpt = false;
            EntityType = typeof(TEntity);
            DeletableEntity = EntityType.GetTypeInfo().ImplementedInterfaces.Any(x => x == typeof(IDeletable));
        }

        /// <summary>
        /// Store base
        /// </summary>
        /// <param name="context"></param>
        /// <param name="includeUnsafeOpt"></param>
        protected StoreBase(TContext context, bool includeUnsafeOpt)
        {
            RawTypedContext = context ?? throw new ArgumentNullException(nameof(context));

            _includeUnsafeOpt = includeUnsafeOpt;
            EntityType = typeof(TEntity);
            DeletableEntity = EntityType.GetTypeInfo().ImplementedInterfaces.Any(x => x == typeof(IDeletable));
        }

        /// <summary>
        /// Include unsafe opt
        /// </summary>
        protected bool IncludeUnsafeOpt => _includeUnsafeOpt;

        /// <summary>
        /// Entity type
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public Type EntityType { get; }

        /// <summary>
        /// To flag this entity type has impl IDelete interface or not.
        /// </summary>
        protected bool DeletableEntity { get; }

        /// <summary>
        /// Context
        /// </summary>
        protected TContext RawTypedContext { get; }

        /// <summary>
        /// Gets typed DbSet
        /// </summary>
        protected DbSet<TEntity> Set => RawTypedContext.Set<TEntity>();

        /// <summary>
        /// No tracking set
        /// </summary>
        protected IQueryable<TEntity> NoTrackingSet => RawTypedContext.Set<TEntity>().AsNoTracking();

        /// <summary>
        /// Find a collection of result by given condition
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate) => Set.Where(predicate);
    }
}