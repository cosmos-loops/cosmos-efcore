using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.Lolita.Update;
using Pomelo.EntityFrameworkCore.Lolita.Delete;
// ReSharper disable SuspiciousTypeConversion.Global

/*
 * reference to:
 *     PomeloFoundation/Lolita
 *     Author: Yuko & PomeloFoundation
 *     URL: https://github.com/PomeloFoundation/Lolita
 *     MIT
 */

namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Extensions for Lolita DbOption
    /// </summary>
    public class LolitaDbOptionExtension : IDbContextOptionsExtension
    {
        /// <summary>
        /// Log fragment
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
                .AddScoped<IFieldParser, DefaultFieldParser>()
                .AddScoped<ISetFieldSqlGenerator, DefaultSetFieldSqlGenerator>()
                .AddScoped<ILolitaUpdateExecutor, DefaultLolitaUpdateExecutor>()
                .AddScoped<ILolitaDeleteExecutor, DefaultLolitaDeleteExecutor>();

            return true;
        }

        /// <summary>
        /// Get service provider hashcode
        /// </summary>
        /// <returns></returns>
        public long GetServiceProviderHashCode()
        {
            return 86216188623901;
        }

        /// <summary>
        /// Validate
        /// </summary>
        /// <param name="options"></param>
        public void Validate(IDbContextOptions options) { }
    }

    /// <summary>
    /// Extensions for lolita DbOption
    /// </summary>
    public static class LolitaDbOptionExtensions
    {
        /// <summary>
        /// Use lolita
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static DbContextOptionsBuilder UseLolita(this DbContextOptionsBuilder self)
        {
            ((IDbContextOptionsBuilderInfrastructure) self).AddOrUpdateExtension(new LolitaDbOptionExtension());
            return self;
        }

        /// <summary>
        /// Use lolita
        /// </summary>
        /// <param name="self"></param>
        /// <typeparam name="TContext"></typeparam>
        /// <returns></returns>
        public static DbContextOptionsBuilder<TContext> UseLolita<TContext>(this DbContextOptionsBuilder<TContext> self) where TContext : DbContext
        {
            ((IDbContextOptionsBuilderInfrastructure) self).AddOrUpdateExtension(new LolitaDbOptionExtension());
            return self;
        }

        /// <summary>
        /// Use lolita
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static DbContextOptions UseLolita(this DbContextOptions self)
        {
            ((IDbContextOptionsBuilderInfrastructure) self).AddOrUpdateExtension(new LolitaDbOptionExtension());
            return self;
        }

        /// <summary>
        /// Use lolita
        /// </summary>
        /// <param name="self"></param>
        /// <typeparam name="TContext"></typeparam>
        /// <returns></returns>
        public static DbContextOptions<TContext> UseLolita<TContext>(this DbContextOptions<TContext> self) where TContext : DbContext
        {
            ((IDbContextOptionsBuilderInfrastructure) self).AddOrUpdateExtension(new LolitaDbOptionExtension());
            return self;
        }
    }
}