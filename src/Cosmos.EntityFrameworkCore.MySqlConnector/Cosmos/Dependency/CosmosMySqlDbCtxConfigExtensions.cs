using System;
using Cosmos.Data.Core.Registrars;
using Cosmos.EntityFrameworkCore;
using Cosmos.EntityFrameworkCore.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cosmos.Dependency
{
    /// <summary>
    /// Cosmos MySQL DbContext configuration extensions.
    /// </summary>
    public static class CosmosMySqlDbContextConfigExtensions
    {
        /// <summary>
        /// Use EntityFramework Core with MySql
        /// </summary>
        /// <param name="context"></param>
        /// <param name="optAct"></param>
        /// <typeparam name="TContext"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static IDbContextConfig UseEfCoreWithMySql<TContext>(this DbContextConfig<MicrosoftProxyRegister> context,
            Action<EfCoreOptions> optAct = null)
            where TContext : DbContext, IEfContext
        {
            return context.UseEfCoreWithMySql<TContext>(null, optAct);
        }

        /// <summary>
        /// Use EntityFramework Core with MySql
        /// </summary>
        /// <param name="context"></param>
        /// <param name="optAct"></param>
        /// <param name="versionProvider"></param>
        /// <typeparam name="TContext"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static IDbContextConfig UseEfCoreWithMySql<TContext>(this DbContextConfig<MicrosoftProxyRegister> context,
            Func<ServerVersion> versionProvider,
            Action<EfCoreOptions> optAct = null)
            where TContext : DbContext, IEfContext
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            var opt = EfCoreOptions.Create(optAct);

            EfCoreOptionsGuard.For(opt);

            context.RegisterDbContext(register =>
            {
                if (!string.IsNullOrWhiteSpace(opt.ConnectionString))
                {
                    EfCoreOptionsRegistrar.Register<TContext>(opt);
                    register.RawServices.AddDbContext<TContext>(builder =>
                    {
                        var connectionString = opt.ConnectionString;
                        var version = versionProvider is null
                            ? ServerVersion.AutoDetect(connectionString)
                            : versionProvider.Invoke();
                        builder.UseMySql(connectionString, version);
                    });
                }
                else if (!string.IsNullOrWhiteSpace(opt.ConnectionName))
                {
                    EfCoreOptionsRegistrar.Register<TContext>(opt);
                    register.RawServices.AddDbContext<TContext>((provider, builder) =>
                    {
                        var connectionString = provider.GetService<IConfigurationRoot>().GetConnectionString(opt.ConnectionName);
                        var version = versionProvider is null
                            ? ServerVersion.AutoDetect(connectionString)
                            : versionProvider.Invoke();
                        builder.UseMySql(connectionString, version);
                    });
                }
                else
                {
                    throw new InvalidOperationException("Connection name or string cannot be empty.");
                }
            });

            return context;
        }

        /// <summary>
        /// Use EntityFramework Core with MySql
        /// </summary>
        /// <param name="context"></param>
        /// <param name="optAct"></param>
        /// <typeparam name="TContextService"></typeparam>
        /// <typeparam name="TContextImplementation"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static IDbContextConfig UseEfCoreWithMySql<TContextService, TContextImplementation>(
            this DbContextConfig<MicrosoftProxyRegister> context, Action<EfCoreOptions> optAct = null)
            where TContextService : IEfContext
            where TContextImplementation : DbContext, TContextService
        {
            return context.UseEfCoreWithMySql<TContextService, TContextImplementation>(null, optAct);
        }

        /// <summary>
        /// Use EntityFramework Core with MySql
        /// </summary>
        /// <param name="context"></param>
        /// <param name="versionProvider"></param>
        /// <param name="optAct"></param>
        /// <typeparam name="TContextService"></typeparam>
        /// <typeparam name="TContextImplementation"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static IDbContextConfig UseEfCoreWithMySql<TContextService, TContextImplementation>(
            this DbContextConfig<MicrosoftProxyRegister> context,
            Func<ServerVersion> versionProvider,
            Action<EfCoreOptions> optAct = null)
            where TContextService : IEfContext
            where TContextImplementation : DbContext, TContextService
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            var opt = EfCoreOptions.Create(optAct);

            EfCoreOptionsGuard.For(opt);

            context.RegisterDbContext(register =>
            {
                if (!string.IsNullOrWhiteSpace(opt.ConnectionString))
                {
                    EfCoreOptionsRegistrar.Register<TContextImplementation>(opt);
                    register.RawServices.AddDbContext<TContextService, TContextImplementation>(
                        builder =>
                        {
                            var connectionString = opt.ConnectionString;
                            var version = versionProvider is null
                                ? ServerVersion.AutoDetect(connectionString)
                                : versionProvider.Invoke();
                            builder.UseMySql(connectionString, version);
                        });
                }
                else if (!string.IsNullOrWhiteSpace(opt.ConnectionName))
                {
                    EfCoreOptionsRegistrar.Register<TContextImplementation>(opt);
                    register.RawServices.AddDbContext<TContextService, TContextImplementation>(
                        (provider, builder) =>
                        {
                            var connectionString = provider.GetService<IConfigurationRoot>().GetConnectionString(opt.ConnectionName);
                            var version = versionProvider is null
                                ? ServerVersion.AutoDetect(connectionString)
                                : versionProvider.Invoke();
                            builder.UseMySql(connectionString, version);
                        });
                }
                else
                {
                    throw new InvalidOperationException("Connection name or string cannot be empty.");
                }
            });

            return context;
        }
    }
}