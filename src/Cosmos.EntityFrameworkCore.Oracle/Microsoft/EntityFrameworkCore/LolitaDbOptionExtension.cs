using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.Lolita.Update;
// ReSharper disable SuspiciousTypeConversion.Global

namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Extensions for Lolita database option for Oracle
    /// </summary>
    public class OracleLolitaDbOptionExtension : IDbContextOptionsExtension
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
                .AddScoped<ISetFieldSqlGenerator, OracleSetFieldSqlGenerator>();

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
        public void Validate(IDbContextOptions options)
        {
        }
    }

    /// <summary>
    /// Extensions for lolita database option
    /// </summary>
    public static class LolitaDbOptionExtensions
    {
        /// <summary>
        /// Use Oracle lolita
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static DbContextOptionsBuilder UseOracleLolita(this DbContextOptionsBuilder self)
        {
            ((IDbContextOptionsBuilderInfrastructure)self).AddOrUpdateExtension(new LolitaDbOptionExtension());
            ((IDbContextOptionsBuilderInfrastructure)self).AddOrUpdateExtension(new OracleLolitaDbOptionExtension());
            return self;
        }

        /// <summary>
        /// Use Oracle lolita
        /// </summary>
        /// <param name="self"></param>
        /// <typeparam name="TContext"></typeparam>
        /// <returns></returns>
        public static DbContextOptionsBuilder<TContext> UseOracleLolita<TContext>(this DbContextOptionsBuilder<TContext> self) where TContext : DbContext
        {
            ((IDbContextOptionsBuilderInfrastructure)self).AddOrUpdateExtension(new LolitaDbOptionExtension());
            ((IDbContextOptionsBuilderInfrastructure)self).AddOrUpdateExtension(new OracleLolitaDbOptionExtension());
            return self;
        }

        /// <summary>
        /// Use Oracle lolita
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static DbContextOptions UseOracleLolita(this DbContextOptions self)
        {
            ((IDbContextOptionsBuilderInfrastructure)self).AddOrUpdateExtension(new LolitaDbOptionExtension());
            ((IDbContextOptionsBuilderInfrastructure)self).AddOrUpdateExtension(new OracleLolitaDbOptionExtension());
            return self;
        }

        /// <summary>
        /// Use Oracle lolita
        /// </summary>
        /// <param name="self"></param>
        /// <typeparam name="TContext"></typeparam>
        /// <returns></returns>
        public static DbContextOptions<TContext> UseOracleLolita<TContext>(this DbContextOptions<TContext> self) where TContext : DbContext
        {
            ((IDbContextOptionsBuilderInfrastructure)self).AddOrUpdateExtension(new LolitaDbOptionExtension());
            ((IDbContextOptionsBuilderInfrastructure)self).AddOrUpdateExtension(new OracleLolitaDbOptionExtension());
            return self;
        }
    }
}
