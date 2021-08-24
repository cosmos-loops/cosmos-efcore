using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Cosmos.Models.Audits
{
    /// <summary>
    /// Cosmos DbContext extensions
    /// </summary>
    public static class EfCoreAuditHistoryExtensions
    {
        /// <summary>
        /// Ensures the automatic history.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="options"></param>
        public static void EnsureAuditHistory(this DbContext context, AuditHistoryOptions options)
        {
            EnsureAuditHistory(context, () => new AuditHistory(), options);
        }

        /// <summary>
        /// Ensures the automatic history.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="createHistoryFactory"></param>
        /// <param name="options"></param>
        /// <typeparam name="TAuditHistory"></typeparam>
        public static void EnsureAuditHistory<TAuditHistory>(this DbContext context,
            Func<TAuditHistory> createHistoryFactory, AuditHistoryOptions options)
            where TAuditHistory : class, IAuditModel, new()
        {
            // Must ToArray() here for excluding the AutoHistory model.
            // Currently, only support Modified and Deleted entity.
            var entries = context.ChangeTracker.Entries().Where(e => e.State == EntityState.Modified || e.State == EntityState.Deleted).ToArray();
            foreach (var entry in entries)
                context.Add(entry.EnsureAuditHistoryImpl(createHistoryFactory, options));
        }

        internal static TAuditHistory EnsureAuditHistoryImpl<TAuditHistory>(this EntityEntry entry,
            Func<TAuditHistory> createHistoryFactory, AuditHistoryOptions options)
            where TAuditHistory : class, IAuditModel, new()
        {
            var history = createHistoryFactory();
            history.TableName = entry.Metadata.GetTableName();

            // Get the mapped properties for the entity type.
            // (include shadow properties, not include navigations & references)
            var properties = entry.Properties;

            switch (entry.State)
            {
                case EntityState.Added:
                    dynamic c = new ExpandoObject();
                    foreach (var prop in properties)
                    {
                        if (prop.Metadata.IsKey() || prop.Metadata.IsForeignKey())
                            continue;

                        ((IDictionary<string, object>)c)[prop.Metadata.Name] = prop.CurrentValue;
                    }

                    history.RowId = entry.PrimaryKey(); //"0";
                    history.Kind = AuditState.Created;
                    history.CurrentValue = options.JsonSerializer?.Serialize(c);
                    break;

                case EntityState.Modified:
                    dynamic originalVal = new ExpandoObject();
                    dynamic currentVal = new ExpandoObject();

                    PropertyValues databaseValues = null;
                    foreach (var prop in properties)
                    {
                        if (prop.IsModified)
                        {
                            if (prop.OriginalValue != null)
                            {
                                if (!prop.OriginalValue.Equals(prop.CurrentValue))
                                {
                                    ((IDictionary<string, object>)originalVal)[prop.Metadata.Name] = prop.OriginalValue;
                                }
                                else
                                {
                                    databaseValues ??= entry.GetDatabaseValues();
                                    var originalValue = databaseValues.GetValue<object>(prop.Metadata.Name);
                                    ((IDictionary<string, object>)originalVal)[prop.Metadata.Name] = originalValue;
                                }
                            }
                            else
                            {
                                ((IDictionary<string, object>)originalVal)[prop.Metadata.Name] = null;
                            }

                            ((IDictionary<string, object>)currentVal)[prop.Metadata.Name] = prop.CurrentValue;
                        }
                    }

                    history.RowId = entry.PrimaryKey();
                    history.Kind = AuditState.Modified;
                    history.OriginalValue = options.JsonSerializer?.Serialize(originalVal);
                    history.CurrentValue = options.JsonSerializer?.Serialize(currentVal);
                    break;
                case EntityState.Deleted:
                    dynamic o = new ExpandoObject();
                    foreach (var prop in properties)
                        ((IDictionary<string, object>)o)[prop.Metadata.Name] = prop.OriginalValue;
                    history.RowId = entry.PrimaryKey();
                    history.Kind = AuditState.Deleted;
                    history.OriginalValue = options.JsonSerializer?.Serialize(o);
                    break;

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
    }
}