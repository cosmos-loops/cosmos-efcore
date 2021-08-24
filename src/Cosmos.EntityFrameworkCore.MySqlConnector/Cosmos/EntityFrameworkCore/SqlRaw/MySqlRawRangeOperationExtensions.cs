using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Cosmos.EntityFrameworkCore.SqlRaw
{
    public static class MySqlRawRangeOperationExtensions
    {
        public static void UseRangeOperations(this DbContextOptionsBuilder optBuilder)
        {
            optBuilder.ReplaceService<IQuerySqlGeneratorFactory, MySqlRawQueryGeneratorFactory>();
        }
    }
}