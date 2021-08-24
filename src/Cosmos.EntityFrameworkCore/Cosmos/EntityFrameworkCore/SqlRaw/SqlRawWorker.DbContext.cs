using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Cosmos.EntityFrameworkCore.SqlRaw
{
    internal static partial class SqlRawWorker
    {
        public static int Execute(DbContext ctx, string sqlText, IReadOnlyDictionary<string, object> parameters)
        {
            return Execute(
                sqlText,
                ctx.Database.GetDbConnection,
                ctx.GetService<IRelationalTypeMappingSource>,
                () => ctx.Database.CurrentTransaction,
                parameters);
        }

        public static Task<int> ExecuteAsync(DbContext ctx, string sqlText, IReadOnlyDictionary<string, object> parameters, CancellationToken cancellationToken = default)
        {
            return ExecuteAsync(
                sqlText,
                ctx.Database.GetDbConnection,
                ctx.GetService<IRelationalTypeMappingSource>,
                () => ctx.Database.CurrentTransaction,
                parameters,
                cancellationToken);
        }
    }
}