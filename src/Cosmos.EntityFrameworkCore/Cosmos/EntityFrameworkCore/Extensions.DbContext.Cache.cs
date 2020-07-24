using System.Linq;
using Cosmos.Collections;
using Microsoft.EntityFrameworkCore;

namespace Cosmos.EntityFrameworkCore
{
    /// <summary>
    /// Extensions for DbContext cache
    /// </summary>
    public static class DbContextCacheExtensions
    {
        /// <summary>
        /// Clear cache
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static int ClearCache(this IEfContext context)
        {
            if (context is DbContextBase ctx)
            {
                var changed = ctx.ChangeTracker.Entries().ForEachItem(entry => entry.State = EntityState.Detached);
                return changed.Count();
            }

            return 0;
        }
    }
}