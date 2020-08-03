using System;
using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;

namespace Cosmos.EntityFrameworkCore.Core
{
    /// <summary>
    /// EntityFramework Core Options registrar.
    /// </summary>
    public static class EfCoreOptionsRegistrar
    {
        private static ConcurrentDictionary<Type, EfCoreOptions> _efCoreOptionsCache;

        static EfCoreOptionsRegistrar()
        {
            _efCoreOptionsCache = new ConcurrentDictionary<Type, EfCoreOptions>();
        }

        /// <summary>
        /// Register
        /// </summary>
        /// <param name="options"></param>
        /// <typeparam name="TContext"></typeparam>
        /// <exception cref="ArgumentException"></exception>
        public static void Register<TContext>(EfCoreOptions options)
            where TContext : DbContext, IEfContext
        {
            var type = typeof(TContext);
            if (_efCoreOptionsCache.ContainsKey(type))
                throw new ArgumentException($"Options has been registered for '{type}'");
            _efCoreOptionsCache.TryAdd(type, options);
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <returns></returns>
        public static EfCoreOptions Get<TContext>()
            where TContext : DbContext, IEfContext
        {
            var type = typeof(TContext);
            if (_efCoreOptionsCache.TryGetValue(type, out var options))
                return options;
            return null;
        }

        /// <summary>
        /// Try get
        /// </summary>
        /// <param name="options"></param>
        /// <typeparam name="TContext"></typeparam>
        /// <returns></returns>
        public static bool TryGet<TContext>(out EfCoreOptions options)
            where TContext : DbContext, IEfContext
        {
            var type = typeof(TContext);
            return _efCoreOptionsCache.TryGetValue(type, out options);
        }
    }
}