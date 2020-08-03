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
    public class SpRawQuery<T> : SqlQueryBase<T>
    {
        private string _storedProcedureName;

        public SpRawQuery(DatabaseFacade databaseFacade, string storedProcedureName, params DbParameter[] sqlParameters)
            : base(databaseFacade, sqlParameters)
        {
            _storedProcedureName = storedProcedureName;
        }
        
        protected override void InitCommand(DbCommand command)
        {
            command.CommandText = _storedProcedureName;
            command.CommandType = System.Data.CommandType.StoredProcedure;
        }
    }
}