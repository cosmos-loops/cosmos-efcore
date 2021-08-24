using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Cosmos.Data.Common;
using Cosmos.EntityFrameworkCore.Internals;
using Cosmos.EntityFrameworkCore.SqlRaw;
using Cosmos.Models;
using Cosmos.Reflection;
using Cosmos.Reflection.Assignment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;

namespace Cosmos.EntityFrameworkCore
{
    public static class EfCoreExtensions
    {
        #region DbContext - ClearCache

        /// <summary>
        /// Clear cache
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static int ClearCache(this IEfContext context)
        {
            context.CheckNull(nameof(context));

            if (context is not DbContextBase ctx)
                return 0;

            var changedItemCount = 0;
            foreach (var entry in ctx.ChangeTracker.Entries())
            {
                entry.State = EntityState.Detached;
                changedItemCount++;
            }

            return changedItemCount;
        }

        #endregion

        #region DbContext - IsInMemory

        /// <summary>
        /// Is in memory
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool IsInMemory(this DbContextBase context)
        {
            return context.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory";
        }

        #endregion

        #region DbContext - EntityType

        /// <summary>
        /// Get EntityType for given type.
        /// </summary>
        /// <param name="context"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static IEntityType GetEntityType<TEntity>(this DbContextBase context) where TEntity : class, IEntity, new()
        {
            return context.Model.FindEntityType(typeof(TEntity));
        }

        /// <summary>
        /// Get EntityType for given type.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public static IEntityType GetEntityType(this DbContextBase context, Type entityType)
        {
            return context.Model.FindEntityType(entityType);
        }

        #endregion

        #region DbContext - TableName

        /// <summary>
        /// Get table name
        /// </summary>
        /// <param name="context"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static string GetTableName<TEntity>(this DbContextBase context) where TEntity : class, IEntity, new()
        {
            return context.GetEntityType<TEntity>()?.GetTableName();
        }

        /// <summary>
        /// Get table name
        /// </summary>
        /// <param name="context"></param>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public static string GetTableName(this DbContextBase context, Type entityType)
        {
            return context.GetEntityType(entityType)?.GetTableName();
        }

        #endregion

        #region DbContext - KeyNames

        /// <summary>
        /// Get key names.
        /// </summary>
        /// <param name="context"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static string[] GetKeyNames<TEntity>(this DbContextBase context) where TEntity : class, IEntity, new()
        {
            return context.GetEntityType<TEntity>()?
                          .GetKeys()
                          .SelectMany(x => x.Properties.Select(y => y.Name))
                          .ToArray() ?? Array.Empty<string>();
        }

        #endregion

        #region DbContext - UnsafeUpdate

        public static FluentUpdateBuilder<TEntity> UnsafeUpdate<TEntity>(this DbContextBase context)
            where TEntity : class, IEntity, new()
        {
            var set = context.Set<TEntity>();
            return new FluentUpdateBuilder<TEntity>(context, set);
        }

        #endregion

        #region DbContext - GetEntityEntry

        /// <summary>
        /// Get entity entry.
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <param name="entity"></param>
        /// <param name="existBefore"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static EntityEntry<TEntity> GetEntityEntry<TEntity>(this DbContext dbCtx, TEntity entity, out bool existBefore)
            where TEntity : class, IEntity
        {
            var type = typeof(TEntity);

            var entityType = dbCtx.Model.FindEntityType(type);

            var keysGetter = entityType.FindPrimaryKey().Properties
                                       .Select(x => x.PropertyInfo.GetValueGetter<TEntity>())
                                       .ToArray();

            var keyValues = keysGetter
                            .Select(x => x?.Invoke(entity))
                            .ToArray();

            var originalEntity = dbCtx.Set<TEntity>().Local
                                      .FirstOrDefault(x => GetEntityKeyValues(keysGetter, x).SequenceEqual(keyValues));

            EntityEntry<TEntity> entityEntry;

            if (null == originalEntity)
            {
                existBefore = false;
                entityEntry = dbCtx.Attach(entity);
            }
            else
            {
                existBefore = true;
                entityEntry = dbCtx.Entry(originalEntity);
                entityEntry.CurrentValues.SetValues(entity);
            }

            return entityEntry;
        }

        private static object[] GetEntityKeyValues<TEntity>(Func<TEntity, object>[] keyValueGetter, TEntity entity)
        {
            var keyValues = keyValueGetter.Select(x => x?.Invoke(entity)).ToArray();
            return keyValues;
        }

        private static Func<T, object> GetValueGetter<T>(this PropertyInfo propertyInfo)
        {
            return StrongTypedCache<T>.PropertyValueGetters.GetOrAdd(propertyInfo, prop =>
            {
                if (!prop.CanRead)
                    return null;
        
                var instance = Expression.Parameter(typeof(T), "i");
                var property = Expression.Property(instance, prop);
                var convert = Expression.TypeAs(property, typeof(object));
                return (Func<T, object>)Expression.Lambda(convert, instance).Compile();
            });
        }

        #endregion

        #region DbSet - GetDbContext

        /// <summary>
        /// Get <see cref="DbContext"/>
        /// </summary>
        /// <param name="dbSet"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static DbContext GetDbContext<TEntity>(this DbSet<TEntity> dbSet) where TEntity : class, IEntity, new()
        {
            return (DbContext)dbSet
                              .GetValueAccessor()
                              .GetValue("_context", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        #endregion

        #region DbSet - GetPrimaryKeyName

        public static string GetPrimaryKeyName<TEntity>(this DbSet<TEntity> dbSet) where TEntity : class, IEntity
        {
            var primaryKeyProperties = dbSet.EntityType.FindPrimaryKey().Properties;
            if (primaryKeyProperties.Count == 0)
                throw new ArgumentException("There's no primary key can be found.");
            if (primaryKeyProperties.Count > 1)
                throw new ArgumentException("Only entity types with one single primary key are supported.");
            return primaryKeyProperties[0].GetColumnName(StoreObjectIdentifier.SqlQuery(dbSet.EntityType));
        }

        #endregion

        #region DbSet - UnsafeRemove

        public static int UnsafeRemove<TEntity>(this DbSet<TEntity> dbSet, Expression<Func<TEntity, bool>> predicate = null, bool ignoreQueryFilters = false)
            where TEntity : class, IEntity, new()
        {
            var ctx = dbSet.GetDbContext();
            var sql = SqlRawDeleteCommandGenerator.Generate(ctx, dbSet, predicate, ignoreQueryFilters, out var parameters);
            return SqlRawWorker.Execute(ctx, sql, parameters);
        }

        public static async Task<int> UnsafeRemoveAsync<TEntity>(this DbSet<TEntity> dbSet,
            Expression<Func<TEntity, bool>> predicate = null, bool ignoreQueryFilters = false, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity, new()
        {
            var ctx = dbSet.GetDbContext();
            var sql = SqlRawDeleteCommandGenerator.Generate(ctx, dbSet, predicate, ignoreQueryFilters, out var parameters);
            return await SqlRawWorker.ExecuteAsync(ctx, sql, parameters, cancellationToken);
        }

        #endregion

        #region DbSet - UnsafeUpdate

        public static FluentUpdateBuilder<TEntity> UnsafeUpdate<TEntity>(this DbSet<TEntity> dbSet)
            where TEntity : class, IEntity, new()
        {
            var context = dbSet.GetDbContext();
            return new FluentUpdateBuilder<TEntity>(context, dbSet);
        }

        #endregion

        #region EntityEntry - GetKeyValues

        public static KeyEntry[] GetKeyValues(this EntityEntry entityEntry)
        {
            if (!entityEntry.IsKeySet)
                return Array.Empty<KeyEntry>();

            return entityEntry.Properties
                              .Where(p => p.Metadata.IsPrimaryKey())
                              .Select(x => new KeyEntry
                              {
                                  PropertyName = x.Metadata.Name,
                                  ColumnName = x.GetColumnName(),
                                  Value = x.CurrentValue,
                              }).ToArray();
        }

        #endregion

        #region Transaction - ToTransactionWrapper

        /// <summary>
        /// To traction wrapper
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public static ITransactionWrapper ToTransactionWrapper(this IDbContextTransaction transaction)
        {
            transaction.CheckNull(nameof(transaction));
            return TransactionWrapper.CreateFromTransaction(transaction.GetDbTransaction());
        }

        /// <summary>
        /// To traction wrapper
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="operator"></param>
        /// <returns></returns>
        public static ITransactionWrapper ToTransactionWrapper(this IDbContextTransaction transaction, TransactionAppendOperator @operator)
        {
            transaction.CheckNull(nameof(transaction));
            return TransactionWrapper.CreateFromTransaction(transaction.GetDbTransaction(), @operator);
        }

        #endregion

        #region Queryable - IncludeIfNeed

        /// <summary>
        /// Include if need...
        /// </summary>
        /// <param name="source"></param>
        /// <param name="condition"></param>
        /// <param name="property"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <returns></returns>
        public static IQueryable<TEntity> IncludeIfNeed<TEntity, TProperty>(
            this IQueryable<TEntity> source, bool condition,
            Expression<Func<TEntity, TProperty>> property) where TEntity : class
        {
            return condition
                ? source.Include(property)
                : source;
        }

        /// <summary>
        /// Include if need...
        /// </summary>
        /// <param name="source"></param>
        /// <param name="condition"></param>
        /// <param name="property"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <returns></returns>
        public static IQueryable<TEntity> IncludeIfNeed<TEntity, TProperty>(
            this IQueryable<TEntity> source, Func<bool> condition,
            Expression<Func<TEntity, TProperty>> property) where TEntity : class
        {
            return condition()
                ? source.Include(property)
                : source;
        }

        #endregion
    }
}