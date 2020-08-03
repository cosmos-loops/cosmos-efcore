using System.Data.Common;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Cosmos.EntityFrameworkCore.Core.RawQuery;

/*
 * Reference to:
 *     https://github.com/PaulARoy/EntityFrameworkCore.RawSQLExtensions
 * Author:
 *     Paul Roy
 */

namespace Cosmos.EntityFrameworkCore
{
    public class SqlRawQuery<T> : SqlQueryBase<T>
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
    }
}