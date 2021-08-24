using System;
using Cosmos.Reflection;
using Cosmos.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Cosmos.Models.Audits
{
    /// <summary>
    /// Cosmos <see cref="ModelBuilder"/> extensions.
    /// </summary>
    public static class EnableAuditExtensions
    {
        private const int DefaultChangedMaxLength = 2048;

        public static ModelBuilder RegisterForAuditHistory(this ModelBuilder modelBuilder, int? historyValueMaxLength = null)
        {
            return RegisterForAuditHistory<AuditHistory>(modelBuilder, o =>
            {
                o.HistoryValueMaxLength = historyValueMaxLength;
                o.LimitHistoryValueLength = false;
            });
        }

        public static ModelBuilder RegisterForAuditHistory<TAuditHistory>(this ModelBuilder modelBuilder, Action<AuditHistoryOptions> optionAct)
            where TAuditHistory : AuditHistory
        {
            var options = AuditHistoryOptions.Default.DeepCopy();
            optionAct?.Invoke(options);
            return modelBuilder.RegisterForAuditHistory<TAuditHistory>(options);
        }

        public static ModelBuilder RegisterForAuditHistory<TAuditHistory>(this ModelBuilder modelBuilder, AuditHistoryOptions options)
            where TAuditHistory : AuditHistory
        {
            options ??= AuditHistoryOptions.Default;

            if (options.JsonSerializer is null)
            {
                options.SerializerConfigureEntry.UseMicrosoftJson();
            }

            modelBuilder.Entity<TAuditHistory>(b =>
            {
                b.ToTable("COSMOS_SYS_AuditHistory"); //Default，下一版将可以自定义
                b.Property(c => c.RowId).IsRequired().HasMaxLength(options.RowIdMaxLength);
                b.Property(c => c.TableName).IsRequired().HasMaxLength(options.TableNameMaxLength);

                if (options.LimitHistoryValueLength)
                {
                    var max = options.HistoryValueMaxLength ?? DefaultChangedMaxLength;
                    if (max <= 0)
                        max = DefaultChangedMaxLength;

                    b.Property(c => c.OriginalValue).HasMaxLength(max);
                    b.Property(c => c.CurrentValue).HasMaxLength(max);
                }
            });

            return modelBuilder;
        }
    }
}