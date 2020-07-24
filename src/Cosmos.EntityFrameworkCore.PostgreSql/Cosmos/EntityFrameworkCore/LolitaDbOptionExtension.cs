using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.Lolita.Update;

// ReSharper disable SuspiciousTypeConversion.Global

namespace Cosmos.EntityFrameworkCore
{
    /// <summary>
    /// Extensions for Lolita database option for PostgreSQL
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class PostgreSQLLolitaDbOptionExtension : IDbContextOptionsExtension
    {
        /// <summary>
        /// Log Fragment
        /// </summary>
        public string LogFragment => "Pomelo.EFCore.Lolita";

        /// <summary>
        /// Apply services
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public bool ApplyServices(IServiceCollection services)
        {
            services
               .AddScoped<ISetFieldSqlGenerator, PostgreSQLSetFieldSqlGenerator>();

            return true;
        }

        /// <summary>
        /// Get service provider hashcode
        /// </summary>
        /// <returns></returns>
        public long GetServiceProviderHashCode()
        {
            return 86216188623903;
        }

        /// <summary>
        /// Validate
        /// </summary>
        /// <param name="options"></param>
        public void Validate(IDbContextOptions options) { }
    }

    /// <summary>
    /// Extensions for lolita database option
    /// </summary>
    public static class LolitaDbOptionExtensions
    {
        /// <summary>
        /// Use PostgreSql lolita
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        public static DbContextOptionsBuilder UsePostgreSQLLolita(this DbContextOptionsBuilder self)
        {
            ((IDbContextOptionsBuilderInfrastructure) self).AddOrUpdateExtension(new LolitaDbOptionExtension());
            ((IDbContextOptionsBuilderInfrastructure) self).AddOrUpdateExtension(new PostgreSQLLolitaDbOptionExtension());
            return self;
        }

        /// <summary>
        /// Use PostgreSql lolita
        /// </summary>
        /// <param name="self"></param>
        /// <typeparam name="TContext"></typeparam>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        public static DbContextOptionsBuilder<TContext> UsePostgreSQLLolita<TContext>(this DbContextOptionsBuilder<TContext> self) where TContext : DbContext
        {
            ((IDbContextOptionsBuilderInfrastructure) self).AddOrUpdateExtension(new LolitaDbOptionExtension());
            ((IDbContextOptionsBuilderInfrastructure) self).AddOrUpdateExtension(new PostgreSQLLolitaDbOptionExtension());
            return self;
        }

        /// <summary>
        /// Use PostgreSql lolita
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        public static DbContextOptions UsePostgreSQLLolita(this DbContextOptions self)
        {
            ((IDbContextOptionsBuilderInfrastructure) self).AddOrUpdateExtension(new LolitaDbOptionExtension());
            ((IDbContextOptionsBuilderInfrastructure) self).AddOrUpdateExtension(new PostgreSQLLolitaDbOptionExtension());
            return self;
        }

        /// <summary>
        /// Use PostgreSql lolita
        /// </summary>
        /// <param name="self"></param>
        /// <typeparam name="TContext"></typeparam>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        public static DbContextOptions<TContext> UsePostgreSQLLolita<TContext>(this DbContextOptions<TContext> self) where TContext : DbContext
        {
            ((IDbContextOptionsBuilderInfrastructure) self).AddOrUpdateExtension(new LolitaDbOptionExtension());
            ((IDbContextOptionsBuilderInfrastructure) self).AddOrUpdateExtension(new PostgreSQLLolitaDbOptionExtension());
            return self;
        }
    }
}