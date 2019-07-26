using System;

namespace Cosmos.EntityFrameworkCore.Core
{
    public static class InternalEfCoreRegistrar
    {
        public static EfCoreOptions GetOptions(Action<EfCoreOptions> optAct = null)
        {
            var opt = new EfCoreOptions();
            optAct?.Invoke(opt);
            return opt;
        }

        public static void GuardEfCoreOptions(EfCoreOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (string.IsNullOrWhiteSpace(options.ConnectionString))
                throw new ArgumentNullException(nameof(options.ConnectionString));
        }
    }
}