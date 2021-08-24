#pragma warning disable EF1001
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Internal;

namespace Cosmos.EntityFrameworkCore.SqlRaw
{
    internal class NpgSqlRawQueryGenerator : NpgsqlQuerySqlGenerator, ISqlRawQueryGenerator
    {
        /// <summary>
        /// if IsForSingleTable=true, ZackQuerySqlGenerator will change the default behavior to capture PredicateSql and so on.
        /// if IsForSingleTable=false, ZackQuerySqlGenerator will use all the implementations of base class
        /// </summary>
        public bool EnableRangeOperation { get; set; }

        /// <summary>
        /// columns of the select statement
        /// </summary>
        public List<string> ProjectionSql { get; } = new();

        /// <summary>
        /// the where clause
        /// </summary>
        public string PredicateSql { get; set; }

        private readonly ISqlGenerationHelper _sqlGenerationHelper;

        public NpgSqlRawQueryGenerator(QuerySqlGeneratorDependencies dependencies, ISqlGenerationHelper sqlGenerationHelper, bool reverseNullOrderingEnabled, Version postgresVersion)
            : base(dependencies, reverseNullOrderingEnabled, postgresVersion)
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