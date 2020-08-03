using System;

namespace Cosmos.EntityFrameworkCore.Core
{
    /// <summary>
    /// Microsoft EntityFramework Core options helper/
    /// </summary>
    public static class EfCoreOptionsHelper
    {
        /// <summary>
        /// Get Microsoft EntityFramework Core options
        /// </summary>
        /// <param name="optAct"></param>
        /// <returns></returns>
        public static EfCoreOptions CreateOptions(Action<EfCoreOptions> optAct = null)
        {
            var opt = new EfCoreOptions();
            optAct?.Invoke(opt);
            return opt;
        }

        /// <summary>
        /// Guard Microsoft EntityFramework Core options options
        /// </summary>
        /// <param name="options"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void GuardOptions(EfCoreOptions options)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            if (string.IsNullOrWhiteSpace(options.ConnectionString))
                throw new ArgumentNullException(nameof(options.ConnectionString));
        }
    }
}