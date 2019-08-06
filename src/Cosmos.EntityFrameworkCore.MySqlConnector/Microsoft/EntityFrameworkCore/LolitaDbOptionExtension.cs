using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.Lolita.Update;
// ReSharper disable SuspiciousTypeConversion.Global

namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Extensions for Lolita database option for MySql
    /// </summary>
    public class MySqlLolitaDbOptionExtension : IDbContextOptionsExtension
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
                .AddScoped<ISetFieldSqlGenerator, MySqlSetFieldSqlGenerator>();

            return true;
        }

        /// <summary>
        /// Get service provider hashcode
        /// </summary>
        /// <returns></returns>
        public long GetServiceProviderHashCode()
        {
            return 86216188623902;
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
        /// Use MySql lolita
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static DbContextOptionsBuilder UseMySqlLolita(this DbContextOptionsBuilder self)
        {
            ((IDbContextOptionsBuilderInfrastructure)self).AddOrUpdateExtension(new LolitaDbOptionExtension());
            ((IDbContextOptionsBuilderInfrastructure)self).AddOrUpdateExtension(new MySqlLolitaDbOptionExtension());
            return self;
        }

        /// <summary>
        /// Use MySql lolita
        /// </summary>
        /// <param name="self"></param>
        /// <typeparam name="TContext"></typeparam>
        /// <returns></returns>
        public static DbContextOptionsBuilder<TContext> UseMySqlLolita<TContext>(this DbContextOptionsBuilder<TContext> self) where TContext : DbContext
        {
            ((IDbContextOptionsBuilderInfrastructure)self).AddOrUpdateExtension(new LolitaDbOptionExtension());
            ((IDbContextOptionsBuilderInfrastructure)self).AddOrUpdateExtension(new MySqlLolitaDbOptionExtension());
            return self;
        }

        /// <summary>
        /// Use MySql lolita
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static DbContextOptions UseMySqlLolita(this DbContextOptions self)
        {
            ((IDbContextOptionsBuilderInfrastructure)self).AddOrUpdateExtension(new LolitaDbOptionExtension());
            ((IDbContextOptionsBuilderInfrastructure)self).AddOrUpdateExtension(new MySqlLolitaDbOptionExtension());
            return self;
        }

        /// <summary>
        /// Use MySql lolita
        /// </summary>
        /// <param name="self"></param>
        /// <typeparam name="TContext"></typeparam>
        /// <returns></returns>
        public static DbContextOptions<TContext> UseMySqlLolita<TContext>(this DbContextOptions<TContext> self) where TContext : DbContext
        {
            ((IDbContextOptionsBuilderInfrastructure)self).AddOrUpdateExtension(new LolitaDbOptionExtension());
            ((IDbContextOptionsBuilderInfrastructure)self).AddOrUpdateExtension(new MySqlLolitaDbOptionExtension());
            return self;
        }
    }
}
