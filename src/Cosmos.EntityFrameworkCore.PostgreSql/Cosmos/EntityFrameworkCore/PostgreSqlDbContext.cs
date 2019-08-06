using System;
using Cosmos.Data.Transaction;
using Cosmos.EntityFrameworkCore.Map;
using Microsoft.EntityFrameworkCore;

namespace Cosmos.EntityFrameworkCore
{
    /// <summary>
    /// Cosmos DbContext for PostgreSql
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public abstract class PostgreSqlDbContext<TContext> : DbContextBase where TContext : DbContext, IDbContext
    {
        // ReSharper disable once StaticMemberInGenericType
        // ReSharper disable once InconsistentNaming
        private static readonly Type _entityMapType = typeof(IPostgreSqlEntityMap);

        /// <summary>
        /// Create a new instance of <see cref="PostgreSqlDbContext{TContext}"/>
        /// </summary>
        /// <param name="options"></param>
        /// <param name="transactionCallingWrapper"></param>
        protected PostgreSqlDbContext(DbContextOptions<TContext> options, ITransactionCallingWrapper transactionCallingWrapper)
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