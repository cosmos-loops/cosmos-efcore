#pragma warning disable EF1001
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Oracle.EntityFrameworkCore.Infrastructure.Internal;

namespace Cosmos.EntityFrameworkCore.SqlRaw
{
    internal class OracleRawQueryGeneratorFactory : IQuerySqlGeneratorFactory
    {
        private readonly ISqlGenerationHelper _sqlGenerationHelper;
        private readonly QuerySqlGeneratorDependencies _dependencies;

        private readonly IOracleOptions _oracleOptions;

        public OracleRawQueryGeneratorFactory(QuerySqlGeneratorDependencies dependencies,
            ISqlGenerationHelper sqlGenerationHelper, IOracleOptions oracleOptions)
        {
            _dependencies = dependencies;
            _sqlGenerationHelper = sqlGenerationHelper;
            _oracleOptions = oracleOptions;
        }

        public QuerySqlGenerator Create()
        {
            return new OracleRawQueryGenerator(_dependencies, _oracleOptions, _sqlGenerationHelper);
        }
    }
}
#pragma warning restore EF1001