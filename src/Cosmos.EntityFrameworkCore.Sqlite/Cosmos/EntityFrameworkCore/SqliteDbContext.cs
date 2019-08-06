using System;
using Cosmos.Data.Transaction;
using Cosmos.EntityFrameworkCore.Map;
using Microsoft.EntityFrameworkCore;

namespace Cosmos.EntityFrameworkCore
{
    /// <summary>
    /// Cosmos DbContext for Sqlite
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public abstract class SqliteDbContext<TContext> : DbContextBase where TContext : DbContext, IDbContext
    {
        // ReSharper disable once StaticMemberInGenericType
        // ReSharper disable once InconsistentNaming
        private static readonly Type _entityMapType = typeof(ISqliteEntityMap);

        /// <summary>
        /// Create a new instance of <see cref="SqliteDbContext{TContext}"/>
        /// </summary>
        /// <param name="options"></param>
        /// <param name="transactionCallingWrapper"></param>
        protected SqliteDbContext(DbContextOptions<TContext> options, ITransactionCallingWrapper transactionCallingWrapper)
            : base(options, transactionCallingWrapper) { }

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
        }
    }
}