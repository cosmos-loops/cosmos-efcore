using System;
using System.Linq;
using System.Collections.Generic;
using Cosmos.Collections;
using Cosmos.Domain.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json.Linq;

namespace Cosmos.EntityFrameworkCore
{
    /// <summary>
    /// Cosmos DbContext extensions
    /// </summary>
    public static class CosmosDbContextExtensions
    {
        #region Cache

        /// <summary>
        /// Clear cache
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static int ClearCache(this IEfContext context)
        {
            if (context is DbContextBase ctx)
            {
                var changed = ctx.ChangeTracker.Entries().ForEachItem(entry => entry.State = EntityState.Detached);
                return changed.Count();
            }

            return 0;
        }

        #endregion

        #region AutoHistory

/*
 * Reference to:
 *     https://github.com/Arch/AutoHistory
 * Author:
 *     Arch Team Organization, https://github.com/Arch
 *
 * Copyright (c) Arch team. All rights reserved.
 */

        /// <summary>
        /// Ensures the automatic history.
        /// </summary>
        /// <param name="context">The context.</param>
        public static void EnsureAutoHistory(this DbContext context)
        {
            EnsureAutoHistory(context, () => new AutoHistory());
        }

        /// <summary>
        /// Ensures the automatic history.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="createHistoryFactory"></param>
        /// <typeparam name="TAutoHistory"></typeparam>
        public static void EnsureAutoHistory<TAutoHistory>(this DbContext context, Func<TAutoHistory> createHistoryFactory)
            where TAutoHistory : AutoHistory
        {
            // Must ToArray() here for excluding the AutoHistory model.
            // Currently, only support Modified and Deleted entity.
            var entries = context.ChangeTracker.Entries().Where(e => e.State == EntityState.Modified || e.State == EntityState.Deleted).ToArray();
            foreach (var entry in entries)
            {
                context.Add(entry.AutoHistory(createHistoryFactory));
            }
        }

        internal static TAutoHistory AutoHistory<TAutoHistory>(this EntityEntry entry, Func<TAutoHistory> createHistoryFactory)
            where TAutoHistory : AutoHistory
        {
            var history = createHistoryFactory();
            history.TableName = entry.Metadata.GetTableName();

            // Get the mapped properties for the entity type.
            // (include shadow properties, not include navigations & references)
            var properties = entry.Properties;

            var formatting = AutoHistoryOptions.Instance.JsonSerializerSettings.Formatting;
            var jsonSerializer = AutoHistoryOptions.Instance.JsonSerializer;
            var json = new JObject();
            switch (entry.State)
            {
                case EntityState.Added:
                    foreach (var prop in properties)
                    {
                        if (prop.Metadata.IsKey() || prop.Metadata.IsForeignKey())
                        {
                            continue;
                        }

                        json[prop.Metadata.Name] = prop.CurrentValue != null
                            ? JToken.FromObject(prop.CurrentValue, jsonSerializer)
                            : JValue.CreateNull();
                    }

                    // REVIEW: what's the best way to set the RowId?
                    history.RowId = "0";
                    history.Kind = EntityState.Added;
                    history.Changed = json.ToString(formatting);
                    break;
                case EntityState.Modified:
                    var bef = new JObject();
                    var aft = new JObject();

                    PropertyValues databaseValues = null;
                    foreach (var prop in properties)
                    {
                        if (prop.IsModified)
                        {
                            if (prop.OriginalValue != null)
                            {
                                if (!prop.OriginalValue.Equals(prop.CurrentValue))
                                {
                                    bef[prop.Metadata.Name] = JToken.FromObject(prop.OriginalValue, jsonSerializer);
                                }
                                else
                                {
                                    databaseValues ??= entry.GetDatabaseValues();
                                    var originalValue = databaseValues.GetValue<object>(prop.Metadata.Name);
                                    bef[prop.Metadata.Name] = originalValue != null
                                        ? JToken.FromObject(originalValue, jsonSerializer)
                                        : JValue.CreateNull();
                                }
                            }
                            else
                            {
                                bef[prop.Metadata.Name] = JValue.CreateNull();
                            }

                            aft[prop.Metadata.Name] = prop.CurrentValue != null
                                ? JToken.FromObject(prop.CurrentValue, jsonSerializer)
                                : JValue.CreateNull();
                        }
                    }

                    json["before"] = bef;
                    json["after"] = aft;

                    history.RowId = entry.PrimaryKey();
                    history.Kind = EntityState.Modified;
                    history.Changed = json.ToString(formatting);
                    break;
                case EntityState.Deleted:
                    foreach (var prop in properties)
                    {
                        json[prop.Metadata.Name] = prop.OriginalValue != null
                            ? JToken.FromObject(prop.OriginalValue, jsonSerializer)
                            : JValue.CreateNull();
                    }

                    history.RowId = entry.PrimaryKey();
                    history.Kind = EntityState.Deleted;
                    history.Changed = json.ToString(formatting);
                    break;
                case EntityState.Detached:
                case EntityState.Unchanged:
                default:
                    throw new NotSupportedException("AutoHistory only support Deleted and Modified entity.");
            }

            return history;
        }

        private static string PrimaryKey(this EntityEntry entry)
        {
            var key = entry.Metadata.FindPrimaryKey();

            var values = new List<object>();
            foreach (var property in key.Properties)
            {
                var value = entry.Property(property.Name).CurrentValue;
                if (value != null)
                {
                    values.Add(value);
                }
            }

            return string.Join(",", values);
        }

        #endregion

        #region EntityType

        /// <summary>
        /// Get EntityType for given type.
        /// </summary>
        /// <param name="context"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static IEntityType GetEntityType<TEntity>(this DbContextBase context)
            where TEntity : class, IEntity, new()
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

        #region TableName

        /// <summary>
        /// Get table name
        /// </summary>
        /// <param name="context"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static string GetTableName<TEntity>(this DbContextBase context)
            where TEntity : class, IEntity, new()
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
    }
}