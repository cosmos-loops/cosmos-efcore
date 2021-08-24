using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cosmos.EntityFrameworkCore.Internals;
using Cosmos.Models;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace Cosmos.EntityFrameworkCore.SqlRaw
{
    public static class MySqlRawInsertExtensions
    {
        private static MySqlBulkCopy BuildSqlBulkCopy<TEntity>(MySqlConnection conn, DbContext context, MySqlTransaction transaction = null) where TEntity : class, IEntity
        {
            var dbSet = context.Set<TEntity>();
            var entityType = dbSet.EntityType;
            var dbProps = entityType.GetColumnInfoColl<TEntity>();

            var bulkCopy = new MySqlBulkCopy(conn, transaction)
            {
                DestinationTableName = entityType.GetTableName() //Schema is not supported by MySQL
            };

            var sourceOrdinal = 0;
            foreach (var dbProp in dbProps)
            {
                var columnName = dbProp.ColumnName;
                bulkCopy.ColumnMappings.Add(new MySqlBulkCopyColumnMapping(sourceOrdinal, columnName));
                sourceOrdinal++;
            }

            return bulkCopy;
        }

        public static async Task BulkInsertAsync<TEntity>(this DbContext dbCtx,
            IEnumerable<TEntity> items, MySqlTransaction transaction = null, CancellationToken cancellationToken = default) where TEntity : class, IEntity
        {
            var conn = dbCtx.Database.GetDbConnection();
            await conn.OpenIfNeededAsync(cancellationToken);
            var dataTable = dbCtx.Set<TEntity>().BuildDataTable(items);
            var bulkCopy = BuildSqlBulkCopy<TEntity>((MySqlConnection)conn, dbCtx, transaction);
            await bulkCopy.WriteToServerAsync(dataTable, cancellationToken);
        }

        public static void BulkInsert<TEntity>(this DbContext dbCtx,
            IEnumerable<TEntity> items, MySqlTransaction transaction = null) where TEntity : class, IEntity
        {
            var conn = dbCtx.Database.GetDbConnection();
            conn.OpenIfNeeded();
            var dataTable = dbCtx.Set<TEntity>().BuildDataTable(items);
            var bulkCopy = BuildSqlBulkCopy<TEntity>((MySqlConnection)conn, dbCtx, transaction);
            bulkCopy.WriteToServer(dataTable);
        }
    }
}