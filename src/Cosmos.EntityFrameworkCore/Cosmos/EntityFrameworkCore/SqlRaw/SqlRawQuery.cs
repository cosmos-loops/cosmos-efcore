using System.Data.Common;
using Microsoft.EntityFrameworkCore.Infrastructure;

/*
 * Reference to:
 *     https://github.com/PaulARoy/EntityFrameworkCore.RawSQLExtensions
 * Author:
 *     Paul Roy
 */

namespace Cosmos.EntityFrameworkCore.SqlRaw
{
    public class SqlRawQuery<T> : SqlRawQueryBase<T>
    {
        private string _sqlQuery;

        public SqlRawQuery(DatabaseFacade databaseFacade, string sqlQuery, params DbParameter[] sqlParameters)
            : base(databaseFacade, sqlParameters)
        {
            _sqlQuery = sqlQuery;
        }

        protected override void InitCommand(DbCommand command)
        {
            command.CommandText = _sqlQuery;
        }

        public static ISqlRawQueryBuilder<T> Of(DatabaseFacade databaseFacade)
        {
            return new SqlRawQueryBuilder<T>(databaseFacade);
        }
    }
}