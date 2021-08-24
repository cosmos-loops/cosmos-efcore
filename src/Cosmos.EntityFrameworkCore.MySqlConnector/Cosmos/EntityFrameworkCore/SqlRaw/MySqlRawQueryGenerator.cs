#pragma warning disable EF1001
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.ExpressionVisitors.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.Internal;

namespace Cosmos.EntityFrameworkCore.SqlRaw
{
    internal class MySqlRawQueryGenerator : MySqlQuerySqlGenerator, ISqlRawQueryGenerator
    {
        /// <summary>
        /// columns of the select statement
        /// </summary>
        private List<string> _projectSql = new();

        /// <summary>
        /// if IsForSingleTable=true, ZackQuerySqlGenerator will change the default behavior to capture PredicateSQL and so on.
        /// if IsForSingleTable=false, ZackQuerySqlGenerator will use all the implementations of base class
        /// </summary>
        public bool EnableRangeOperation { get; set; }

        public List<string> ProjectionSql => _projectSql;

        /// <summary>
        /// the where clause
        /// </summary>
        public string PredicateSql { get; set; }

        private readonly ISqlGenerationHelper _sqlGenerationHelper;

        public MySqlRawQueryGenerator(QuerySqlGeneratorDependencies dependencies, ISqlGenerationHelper sqlGenerationHelper, MySqlSqlExpressionFactory sqlExpressionFactory, IMySqlOptions options)
            : base(dependencies, sqlExpressionFactory, options)
        {
            _sqlGenerationHelper = sqlGenerationHelper;
            EnableRangeOperation = false;
        }

        protected override Expression VisitSelect(SelectExpression selectExpression)
        {
            if (!EnableRangeOperation)
            {
                return base.VisitSelect(selectExpression);
            }

            return SqlRawGeneratorUtils.VisitSelect(this, _sqlGenerationHelper, selectExpression);
        }

        protected override Expression VisitColumn(ColumnExpression columnExpression)
        {
            if (EnableRangeOperation)
            {
                Sql.Append(_sqlGenerationHelper.DelimitIdentifier(columnExpression.Name));
                return columnExpression;
            }

            return base.VisitColumn(columnExpression);
        }

        protected override Expression VisitTable(TableExpression tableExpression)
        {
            if (EnableRangeOperation)
            {
                Sql.Append(_sqlGenerationHelper.DelimitIdentifier(tableExpression.Name));
                return tableExpression;
            }

            return base.VisitTable(tableExpression);
        }

        /// <inheritdoc />
        public IRelationalCommandBuilder LocalSql => Sql;

        /// <inheritdoc />
        public void LocalGenerateSetOperation(SetOperationBase setOperation) => GenerateSetOperation(setOperation);

        /// <inheritdoc />
        public void LocalGenerateTop(SelectExpression selectExpression) => GenerateTop(selectExpression);

        /// <inheritdoc />
        public void LocalGeneratePseudoFromClause() => GeneratePseudoFromClause();

        /// <inheritdoc />
        public void LocalGenerateOrderings(SelectExpression selectExpression) => GenerateOrderings(selectExpression);

        /// <inheritdoc />
        public void LocalGenerateLimitOffset(SelectExpression selectExpression) => GenerateLimitOffset(selectExpression);

        /// <inheritdoc />
        public string LocalAliasSeparator => AliasSeparator;
    }
}
#pragma warning restore EF1001