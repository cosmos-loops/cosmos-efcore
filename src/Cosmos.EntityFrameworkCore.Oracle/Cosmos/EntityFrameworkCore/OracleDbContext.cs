using System;
using Cosmos.Data.Transaction;
using Cosmos.EntityFrameworkCore.Map;
using Microsoft.EntityFrameworkCore;

namespace Cosmos.EntityFrameworkCore
{
    public abstract class OracleDbContext<TContext> : DbContextBase where TContext : DbContext, IDbContext
    {
        // ReSharper disable once StaticMemberInGenericType
        // ReSharper disable once InconsistentNaming
        private static readonly Type _entityMapType = typeof(IOracleEntityMap);

        protected OracleDbContext(DbContextOptions<TContext> options, ITransactionCallingWrapper transactionCallingWrapper)
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