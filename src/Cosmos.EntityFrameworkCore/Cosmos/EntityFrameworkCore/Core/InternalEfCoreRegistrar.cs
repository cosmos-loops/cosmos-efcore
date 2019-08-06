using System;

namespace Cosmos.EntityFrameworkCore.Core
{
    /// <summary>
    /// Internal efcore registrar
    /// </summary>
    public static class InternalEfCoreRegistrar
    {
        /// <summary>
        /// Get options
        /// </summary>
        /// <param name="optAct"></param>
        /// <returns></returns>
        public static EfCoreOptions GetOptions(Action<EfCoreOptions> optAct = null)
        {
            var opt = new EfCoreOptions();
            optAct?.Invoke(opt);
            return opt;
        }

        /// <summary>
        /// Guard efcore options
        /// </summary>
        /// <param name="options"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void GuardEfCoreOptions(EfCoreOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (string.IsNullOrWhiteSpace(options.ConnectionString))
                throw new ArgumentNullException(nameof(options.ConnectionString));
        }
    }
}