using System;
using Cosmos.Data.Context;
using Cosmos.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cosmos.Data
{
    /// <summary>
    /// Extensions for Cosmos DbContext
    /// </summary>
    public static class DbContextConfigExtensions
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
        public static IDbContextConfig UseEfCoreWithMySql<TContext>(this DbContextConfig context, Action<EfCoreOptions> optAct = null)
            where TContext : DbContext, EntityFrameworkCore.IDbContext
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var opt = new EfCoreOptions();
            optAct?.Invoke(opt);

            context.RegisterDbContext(services =>
            {
                if (!string.IsNullOrWhiteSpace(opt.ConnectionString))
                {
                    services.AddDbContext<TContext>(o => o.UseMySql(opt.ConnectionString).UseLolita());
                }
                else if (!string.IsNullOrWhiteSpace(opt.ConnectionName))
                {
                    services.AddDbContext<TContext>((p, o) =>
                        o.UseMySql(p.GetService<IConfigurationRoot>().GetConnectionString(opt.ConnectionName)).UseLolita());
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
            this DbContextConfig context, Action<EfCoreOptions> optAct = null)
            where TContextService : EntityFrameworkCore.IDbContext
            where TContextImplementation : DbContext, TContextService
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var opt = new EfCoreOptions();
            optAct?.Invoke(opt);

            context.RegisterDbContext(services =>
            {
                if (!string.IsNullOrWhiteSpace(opt.ConnectionString))
                {
                    services.AddDbContext<TContextService, TContextImplementation>(o => o.UseMySql(opt.ConnectionString).UseLolita());
                }
                else if (!string.IsNullOrWhiteSpace(opt.ConnectionName))
                {
                    services.AddDbContext<TContextService, TContextImplementation>((p, o) =>
                        o.UseMySql(p.GetService<IConfigurationRoot>().GetConnectionString(opt.ConnectionName)).UseLolita());
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