using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cosmos.EntityFrameworkCore.Internals;
using Cosmos.Models;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;

namespace Cosmos.EntityFrameworkCore.SqlRaw
{
    public static class OracleRawInsertExtensions
    {
        private static OracleBulkCopy BuildSqlBulkCopy<TEntity>(OracleConnection conn, DbContext dbCtx) where TEntity : class, IEntity
        {
            var dbSet = dbCtx.Set<TEntity>();
            var entityType = dbSet.EntityType;
            var dbProps = entityType.GetColumnInfoColl<TEntity>();

            var bulkCopy = new OracleBulkCopy(conn);

            bulkCopy.DestinationTableName = entityType.GetTableName(); //Schema is not supported by MySQL
            foreach (var dbProp in dbProps)
            {
                var columnName = dbProp.ColumnName;
                bulkCopy.ColumnMappings.Add(columnName, columnName);
            }

            return bulkCopy;
        }

        public static void BulkInsert<TEntity>(this DbContext dbCtx, IEnumerable<TEntity> items) where TEntity : class, IEntity
        {
            var conn = dbCtx.Database.GetDbConnection();
            conn.OpenIfNeeded();
            var dataTable = dbCtx.Set<TEntity>().BuildDataTable(items);
            using (var bulkCopy = BuildSqlBulkCopy<TEntity>((OracleConnection)conn, dbCtx))
            {
                bulkCopy.WriteToServer(dataTable);
            }
        }

        public static async Task BulkInsertAsync<TEntity>(this DbContext dbCtx,
            IEnumerable<TEntity> items, CancellationToken cancellationToken = default) where TEntity : class, IEntity
        {
            var conn = dbCtx.Database.GetDbConnection();
            await conn.OpenIfNeededAsync(cancellationToken);
            var dataTable = dbCtx.Set<TEntity>().BuildDataTable(items);
            using (var bulkCopy = BuildSqlBulkCopy<TEntity>((OracleConnection)conn, dbCtx))
            {
                bulkCopy.WriteToServer(dataTable);
            }
        }
    }
}