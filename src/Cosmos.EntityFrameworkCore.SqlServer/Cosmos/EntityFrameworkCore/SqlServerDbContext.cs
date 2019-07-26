using System;
using Cosmos.Data.Transaction;
using Cosmos.EntityFrameworkCore.Map;
using Microsoft.EntityFrameworkCore;

namespace Cosmos.EntityFrameworkCore
{
    public abstract class SqlServerDbContext<TContext> : DbContextBase where TContext : DbContext, IDbContext
    {
        // ReSharper disable once StaticMemberInGenericType
        // ReSharper disable once InconsistentNaming
        private static readonly Type _entityMapType = typeof(ISqlServerEntityMap);

        protected SqlServerDbContext(DbContextOptions<TContext> options, ITransactionCallingWrapper transactionCallingWrapper)
            : base(options, transactionCallingWrapper) { }

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