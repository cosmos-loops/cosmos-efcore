// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using Cosmos.Domain.Core;

/*
 * Reference to
 *     https://github.com/zzzprojects/EntityFramework-Plus/
 *     Author: zzzprojects
 *     MIT
 */

namespace Microsoft.EntityFrameworkCore
{
    public static partial class DbContextUtils
    {
        public static DbContext GetDbContext<TEntity>(this DbSet<TEntity> dbSet) where TEntity : class, IEntity, new()
        {
            return dbSet.GetType().GetField("_context", _bindingFlags).GetValue<DbContext>(dbSet);
        }
    }
}