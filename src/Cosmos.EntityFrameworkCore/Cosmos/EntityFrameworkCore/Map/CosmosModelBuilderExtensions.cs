using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;

namespace Cosmos.EntityFrameworkCore.Map
{
    /// <summary>
    /// Cosmos <see cref="ModelBuilder"/> extensions.
    /// </summary>
    public static class CosmosModelBuilderExtensions
    {
        #region AutoHistory

/*
 * Reference to:
 *     https://github.com/Arch/AutoHistory
 * Author:
 *     Arch Team Organization, https://github.com/Arch
 *
 * Copyright (c) Arch team. All rights reserved.
 */

        private const int DefaultChangedMaxLength = 2048;

        /// <summary>
        /// Enables the automatic recording change history.
        /// </summary>
        /// <param name="modelBuilder">The <see cref="ModelBuilder"/> to enable auto history feature.</param>
        /// <param name="changedMaxLength">The maximum length of the 'Changed' column. <c>null</c> will use default setting 2048.</param>
        /// <returns>The <see cref="ModelBuilder"/> had enabled auto history feature.</returns>
        public static ModelBuilder EnableAutoHistory(this ModelBuilder modelBuilder, int? changedMaxLength)
        {
            return modelBuilder.EnableAutoHistory<AutoHistory>(o =>
            {
                o.ChangedMaxLength = changedMaxLength;
                o.LimitChangedLength = false;
            });
        }

        public static ModelBuilder EnableAutoHistory<TAutoHistory>(this ModelBuilder modelBuilder, Action<AutoHistoryOptions> configure)
            where TAutoHistory : AutoHistory
        {
            var options = AutoHistoryOptions.Instance;
            configure?.Invoke(options);
            options.JsonSerializer = JsonSerializer.Create(options.JsonSerializerSettings);

            modelBuilder.Entity<TAutoHistory>(b =>
            {
                b.Property(c => c.RowId).IsRequired().HasMaxLength(options.RowIdMaxLength);
                b.Property(c => c.TableName).IsRequired().HasMaxLength(options.TableMaxLength);

                if (options.LimitChangedLength)
                {
                    var max = options.ChangedMaxLength ?? DefaultChangedMaxLength;
                    if (max <= 0)
                    {
                        max = DefaultChangedMaxLength;
                    }

                    b.Property(c => c.Changed).HasMaxLength(max);
                }

                // This MSSQL only
                //b.Property(c => c.Created).HasDefaultValueSql("getdate()");
            });

            return modelBuilder;
        }

        #endregion
    }
}