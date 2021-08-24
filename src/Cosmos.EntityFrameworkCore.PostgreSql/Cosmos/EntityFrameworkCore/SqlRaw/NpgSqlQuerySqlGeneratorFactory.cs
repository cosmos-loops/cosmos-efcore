#pragma warning disable EF1001
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;

namespace Cosmos.EntityFrameworkCore.SqlRaw
{
    internal class NpgSqlRawQueryGeneratorFactory : IQuerySqlGeneratorFactory
    {
        private readonly QuerySqlGeneratorDependencies _dependencies;
        private readonly ISqlGenerationHelper _sqlGenerationHelper;
        readonly INpgsqlOptions _npgsqlOptions;

        public NpgSqlRawQueryGeneratorFactory(QuerySqlGeneratorDependencies dependencies,
            ISqlGenerationHelper sqlGenerationHelper, INpgsqlOptions npgsqlOptions)
        {
            _dependencies = dependencies;
            _sqlGenerationHelper = sqlGenerationHelper;
            _npgsqlOptions = npgsqlOptions;
        }

        public QuerySqlGenerator Create()
        {
            return new NpgSqlRawQueryGenerator(_dependencies, _sqlGenerationHelper,
                _npgsqlOptions.ReverseNullOrderingEnabled,
                _npgsqlOptions.PostgresVersion);
        }
    }
}
#pragma warning restore EF1001