#pragma warning disable EF1001
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Internal;

namespace Cosmos.EntityFrameworkCore.SqlRaw
{
    public static class NpgSqlRawRangeOperationExtensions
    {
        public static void UseRangeOperations(this DbContextOptionsBuilder optBuilder)
        {
            optBuilder.ReplaceService<IQuerySqlGeneratorFactory, NpgsqlQuerySqlGeneratorFactory>();
        }
    }
}
#pragma warning restore EF1001