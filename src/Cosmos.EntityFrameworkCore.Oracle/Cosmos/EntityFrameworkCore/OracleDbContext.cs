using System;
using Cosmos.EntityFrameworkCore.Core;
using Cosmos.EntityFrameworkCore.Map;
using Microsoft.EntityFrameworkCore;

namespace Cosmos.EntityFrameworkCore
{
    /// <summary>
    /// Cosmos DbContext for Oracle
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public abstract class OracleDbContext<TContext> : DbContextBase where TContext : DbContext, IEfContext
    {
        // ReSharper disable once StaticMemberInGenericType
        // ReSharper disable once InconsistentNaming
        private static readonly Type _entityMapType = typeof(IOracleEntityMap);

        /// <summary>
        /// Create a new instance of <see cref="OracleDbContext{TContext}"/>
        /// </summary>
        /// <param name="options"></param>
        protected OracleDbContext(DbContextOptions<TContext> options)
            : base(options, EfCoreOptionsRegistrar.Get<TContext>())
        {
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

            //if AutoHistory 
            if (OwnEfCoreOptions.AutoHistory.Enable)
            {
                modelBuilder.EnableAutoHistory<AutoHistory>(OwnEfCoreOptions.AutoHistory.ToConfigure());
            }
        }
    }
}