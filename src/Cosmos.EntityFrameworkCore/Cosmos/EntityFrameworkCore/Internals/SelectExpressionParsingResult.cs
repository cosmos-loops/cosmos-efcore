using System.Collections.Generic;
using Cosmos.EntityFrameworkCore.SqlRaw;

/*
 * Reference: Zack.EFCore.Batch
 *   url: https://github.com/yangzhongke/Zack.EFCore.Batch
 *   author: 杨中科
 */

namespace Cosmos.EntityFrameworkCore.Internals
{
    internal class ParsedSelectResult
    {
        /// <summary>
        /// parameters of query
        /// </summary>
        public IReadOnlyDictionary<string, object> Parameters { get; set; }

        /// <summary>
        /// Schema
        /// </summary>
        public string Schema { get; set; }

        /// <summary>
        /// Table name
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// columns of select
        /// </summary>
        public IEnumerable<string> ProjectionSql { get; set; }

        /// <summary>
        /// where clause
        /// </summary>
        public string PredicateSql { get; set; }

        /// <summary>
        /// Full Sql
        /// </summary>
        public string FullSql { get; set; }

        /// <summary>
        /// Query Sql Generator
        /// </summary>
        public ISqlRawQueryGenerator QuerySqlGenerator { get; set; }
    }
}