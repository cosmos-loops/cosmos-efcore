using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.Lolita.Update;

// ReSharper disable SuspiciousTypeConversion.Global

namespace Cosmos.EntityFrameworkCore
{
    /// <summary>
    /// Extensions for Lolita database option for SqlServer
    /// </summary>
    public class SqlServerLolitaDbOptionExtension : IDbContextOptionsExtension
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
               .AddScoped<ISetFieldSqlGenerator, SqlServerSetFieldSqlGenerator>();

            return true;
        }

        /// <summary>
        /// Get service provider hashcode
        /// </summary>
        /// <returns></returns>
        public long GetServiceProviderHashCode()
        {
            return 86216188623904;
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
        /// Use SqlServer lolita
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static DbContextOptionsBuilder UseSqlServerLolita(this DbContextOptionsBuilder self)
        {
            ((IDbContextOptionsBuilderInfrastructure) self).AddOrUpdateExtension(new LolitaDbOptionExtension());
            ((IDbContextOptionsBuilderInfrastructure) self).AddOrUpdateExtension(new SqlServerLolitaDbOptionExtension());
            return self;
        }

        /// <summary>
        /// Use SqlServer lolita
        /// </summary>
        /// <param name="self"></param>
        /// <typeparam name="TContext"></typeparam>
        /// <returns></returns>
        public static DbContextOptionsBuilder<TContext> UseSqlServerLolita<TContext>(this DbContextOptionsBuilder<TContext> self) where TContext : DbContext
        {
            ((IDbContextOptionsBuilderInfrastructure) self).AddOrUpdateExtension(new LolitaDbOptionExtension());
            ((IDbContextOptionsBuilderInfrastructure) self).AddOrUpdateExtension(new SqlServerLolitaDbOptionExtension());
            return self;
        }

        /// <summary>
        /// Use SqlServer lolita
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static DbContextOptions UseSqlServerLolita(this DbContextOptions self)
        {
            ((IDbContextOptionsBuilderInfrastructure) self).AddOrUpdateExtension(new LolitaDbOptionExtension());
            ((IDbContextOptionsBuilderInfrastructure) self).AddOrUpdateExtension(new SqlServerLolitaDbOptionExtension());
            return self;
        }

        /// <summary>
        /// Use SqlServer lolita
        /// </summary>
        /// <param name="self"></param>
        /// <typeparam name="TContext"></typeparam>
        /// <returns></returns>
        public static DbContextOptions<TContext> UseSqlServerLolita<TContext>(this DbContextOptions<TContext> self) where TContext : DbContext
        {
            ((IDbContextOptionsBuilderInfrastructure) self).AddOrUpdateExtension(new LolitaDbOptionExtension());
            ((IDbContextOptionsBuilderInfrastructure) self).AddOrUpdateExtension(new SqlServerLolitaDbOptionExtension());
            return self;
        }
    }
}