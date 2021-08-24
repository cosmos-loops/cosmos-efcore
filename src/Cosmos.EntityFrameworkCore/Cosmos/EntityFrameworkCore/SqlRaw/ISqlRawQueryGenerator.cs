using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace Cosmos.EntityFrameworkCore.SqlRaw
{
    public interface ISqlRawQueryGenerator
    {
        public bool EnableRangeOperation { get; set; }

        /// <summary>
        /// Gets the project Sql.
        /// </summary>
        public List<string> ProjectionSql { get; }

        /// <summary>
        /// Gets or sets the condition sql
        /// </summary>
        public string PredicateSql { get; set; }

        /// <summary>
        /// Get relational command by the given SelectExpression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IRelationalCommand GetCommand(SelectExpression expression);

        public IRelationalCommandBuilder LocalSql { get; }

        public Expression Visit(Expression node);

        public void LocalGenerateSetOperation(SetOperationBase setOperation);

        public void LocalGenerateTop(SelectExpression selectExpression);

        public void LocalGeneratePseudoFromClause();

        public void LocalGenerateOrderings(SelectExpression selectExpression);

        public void LocalGenerateLimitOffset(SelectExpression selectExpression);

        public string LocalAliasSeparator { get; }
    }
}