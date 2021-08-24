using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;

namespace Cosmos.EntityFrameworkCore.SqlRaw
{
    internal class SqliteRawQueryGeneratorFactory : IQuerySqlGeneratorFactory
    {
        private readonly QuerySqlGeneratorDependencies _dependencies;
        private readonly ISqlGenerationHelper _sqlGenerationHelper;

        public SqliteRawQueryGeneratorFactory(QuerySqlGeneratorDependencies dependencies,
            ISqlGenerationHelper sqlGenerationHelper)
        {
            _dependencies = dependencies;
            _sqlGenerationHelper = sqlGenerationHelper;
        }

        public QuerySqlGenerator Create()
        {
            return new SqliteRawQueryGenerator(_dependencies, _sqlGenerationHelper);
        }
    }
}