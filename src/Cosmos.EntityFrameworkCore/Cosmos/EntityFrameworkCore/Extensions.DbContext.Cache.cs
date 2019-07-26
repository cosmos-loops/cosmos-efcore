using System.Linq;
using Cosmos.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Cosmos.EntityFrameworkCore
{
    public static class DbContextCacheExtensions
    {
        public static int ClearCache(this IDbContext context)
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