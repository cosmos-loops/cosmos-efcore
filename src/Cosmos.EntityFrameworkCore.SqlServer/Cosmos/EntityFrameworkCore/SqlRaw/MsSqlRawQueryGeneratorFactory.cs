using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;

namespace Cosmos.EntityFrameworkCore.SqlRaw
{
    internal class MsSqlRawQueryGeneratorFactory : IQuerySqlGeneratorFactory
    {
        private readonly QuerySqlGeneratorDependencies _dependencies;

        private readonly ISqlGenerationHelper _sqlGenerationHelper;

        public MsSqlRawQueryGeneratorFactory(QuerySqlGeneratorDependencies dependencies, ISqlGenerationHelper sqlGenerationHelper)
        {
            _dependencies = dependencies;
            _sqlGenerationHelper = sqlGenerationHelper;
        }

        public QuerySqlGenerator Create()
        {
            return new MsSqlRawQueryGenerator(_dependencies, _sqlGenerationHelper);
        }
    }
}