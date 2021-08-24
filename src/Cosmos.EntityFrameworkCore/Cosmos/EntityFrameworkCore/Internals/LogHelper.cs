#pragma warning disable EF1001

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Diagnostics.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;

/*
 * Reference: Zack.EFCore.Batch
 *   url: https://github.com/yangzhongke/Zack.EFCore.Batch
 *   author: 杨中科
 */

namespace Cosmos.EntityFrameworkCore.Internals
{
    static class LogHelper
    {
        public static void Log(this DbContext dbContext, string msg)
        {
            var logger = dbContext.GetService<IDiagnosticsLogger<DbLoggerCategory.Update>>();
            EventDefinitionBase eventData = CoreResources.LogQueryExecutionPlanned(logger);
            logger.DbContextLogger.Log(new EventData(eventData, (_, _) => msg));
        }
    }
}
#pragma warning restore EF1001