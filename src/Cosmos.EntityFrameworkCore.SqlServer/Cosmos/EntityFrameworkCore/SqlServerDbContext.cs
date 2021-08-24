using System;
using Cosmos.EntityFrameworkCore.Core;
using Cosmos.EntityFrameworkCore.EntityMapping;
using Cosmos.EntityFrameworkCore.SqlRaw;
using Cosmos.Models.Audits;
using Microsoft.EntityFrameworkCore;

namespace Cosmos.EntityFrameworkCore
{
    /// <summary>
    /// Cosmos DbContext for SqlServer
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public abstract class SqlServerDbContext<TContext> : DbContextBase where TContext : DbContext, IEfContext
    {
        // ReSharper disable once StaticMemberInGenericType
        // ReSharper disable once InconsistentNaming
        private static readonly Type _entityMapType = typeof(ISqlServerEntityMap);

        /// <summary>
        /// Create a new instance of <see cref="SqlServerDbContext{TContext}"/>
        /// </summary>
        /// <param name="options"></param>
        protected SqlServerDbContext(DbContextOptions<TContext> options)
            : base(options, EfCoreOptionsRegistrar.Get<TContext>()) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            //if EnableRangeOperation
            if (OwnEfCoreOptions.EnableRangeOperation)
            {
                optionsBuilder.UseRangeOperations();
            }
        }

        /// <summary>
        /// On model creating
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            using (var scanner = new EntityMapScanner<TContext>(_entityMapType))
            {
                foreach (var map in scanner.ScanAndReturnInstances())
                {
                    map?.Map(modelBuilder);
                }
            }

            //if EnableAudit 
            if (OwnEfCoreOptions.EnableAudit)
            {
                modelBuilder.RegisterForAuditHistory<AuditHistory>(OwnEfCoreOptions.AuditHistoryOptions);
            }
        }
    }
}