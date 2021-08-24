#pragma warning disable EF1001
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.Internal;

namespace Cosmos.EntityFrameworkCore.SqlRaw
{
    internal class MySqlRawQueryGeneratorFactory : IQuerySqlGeneratorFactory
    {
        private readonly ISqlGenerationHelper _sqlGenerationHelper;
        private readonly QuerySqlGeneratorDependencies _dependencies;
        private readonly MySqlSqlExpressionFactory _sqlExpressionFactory;

        private readonly IMySqlOptions _options;

        public MySqlRawQueryGeneratorFactory(QuerySqlGeneratorDependencies dependencies,
            ISqlGenerationHelper sqlGenerationHelper, ISqlExpressionFactory sqlExpressionFactory, IMySqlOptions options)
        {
            _dependencies = dependencies;
            _sqlGenerationHelper = sqlGenerationHelper;
            _sqlExpressionFactory = (MySqlSqlExpressionFactory)sqlExpressionFactory;
            _options = options;
        }

        public QuerySqlGenerator Create()
        {
            return new MySqlRawQueryGenerator(_dependencies, _sqlGenerationHelper, _sqlExpressionFactory, _options);
        }
    }
}
#pragma warning restore EF1001