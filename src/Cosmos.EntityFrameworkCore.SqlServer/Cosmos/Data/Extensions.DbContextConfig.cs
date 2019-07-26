using System;
using Cosmos.Data.Context;
using Cosmos.EntityFrameworkCore;
using Cosmos.EntityFrameworkCore.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using IEfCoreDbContext = Cosmos.EntityFrameworkCore.IDbContext;

namespace Cosmos.Data
{
    public static class DbContextConfigExtensions
    {
        public static IDbContextConfig UseEfCoreWithSqlServer<TContext>(
            this DbContextConfig config, Action<EfCoreOptions> optAct = null)
            where TContext : DbContext, IEfCoreDbContext
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            var options = InternalEfCoreRegistrar.GetOptions(optAct);

            return CoreRegistrar.Register(config, options,
                s => s.AddDbContext<TContext>(o => o.UseSqlServer(options.ConnectionString).UseLolita()));
        }

        public static IDbContextConfig UseEfCoreWithSqlServer<TCtxtService, TCtxImplementation>(
            this DbContextConfig config, Action<EfCoreOptions> optAct = null)
            where TCtxtService : IEfCoreDbContext
            where TCtxImplementation : DbContext, TCtxtService
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            var options = InternalEfCoreRegistrar.GetOptions(optAct);

            return CoreRegistrar.Register(config, options,
                s => s.AddDbContext<TCtxtService, TCtxImplementation>(o => o.UseSqlServer(options.ConnectionString).UseLolita()));
        }

        private static class CoreRegistrar
        {
            public static DbContextConfig Register(DbContextConfig context, EfCoreOptions options, Action<IServiceCollection> action)
            {
                InternalEfCoreRegistrar.GuardEfCoreOptions(options);
                context.RegisterDbContext(services => { action?.Invoke(services); });
                return context;
            }
        }
    }
}