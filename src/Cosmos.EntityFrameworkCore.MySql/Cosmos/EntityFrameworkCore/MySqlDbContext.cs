using System;
using Cosmos.EntityFrameworkCore.Map;
using Microsoft.EntityFrameworkCore;

namespace Cosmos.EntityFrameworkCore
{
    /// <summary>
    /// Cosmos DbContext for MySql
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public abstract class MySqlDbContext<TContext> : DbContextBase where TContext : DbContext, IEfContext
    {
        // ReSharper disable once StaticMemberInGenericType
        // ReSharper disable once InconsistentNaming
        private static readonly Type _entityMapType = typeof(IMySqlEntityMap);

        /// <summary>
        /// Create a new instance of <see cref="MySqlDbContext{TContext}"/>
        /// </summary>
        /// <param name="options"></param>
        protected MySqlDbContext(DbContextOptions<TContext> options) : base(options) { }

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