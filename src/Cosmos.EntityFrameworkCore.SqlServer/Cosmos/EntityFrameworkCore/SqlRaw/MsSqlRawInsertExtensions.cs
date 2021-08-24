using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cosmos.EntityFrameworkCore.Internals;
using Cosmos.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Cosmos.EntityFrameworkCore.SqlRaw
{
    public static class MsSqlRawInsertExtensions
    {
        public static void BulkInsert<TEntity>(this DbContext dbCtx, IEnumerable<TEntity> items)
            where TEntity : class, IEntity
        {
            var conn = dbCtx.Database.GetDbConnection();
            conn.OpenIfNeeded();
            var dataTable = dbCtx.Set<TEntity>().BuildDataTable(items);
            using (var bulkCopy = BuildSqlBulkCopy<TEntity>((SqlConnection)conn, dbCtx))
            {
                bulkCopy.WriteToServer(dataTable);
            }
        }

        public static async Task BulkInsertAsync<TEntity>(this DbContext dbCtx,
            IEnumerable<TEntity> items, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            var conn = dbCtx.Database.GetDbConnection();
            await conn.OpenIfNeededAsync(cancellationToken);
            var dataTable = dbCtx.Set<TEntity>().BuildDataTable(items);
            using (var bulkCopy = BuildSqlBulkCopy<TEntity>((SqlConnection)conn, dbCtx))
            {
                await bulkCopy.WriteToServerAsync(dataTable, cancellationToken);
            }
        }

        private static SqlBulkCopy BuildSqlBulkCopy<TEntity>(SqlConnection conn, DbContext dbCtx)
            where TEntity : class, IEntity
        {
            var bulkCopy = new SqlBulkCopy(conn);
            var dbSet = dbCtx.Set<TEntity>();
            var entityType = dbSet.EntityType;
            var dbProps = entityType.GetColumnInfoColl<TEntity>();
            bulkCopy.DestinationTableName = entityType.GetSchemaQualifiedTableName(); //Schema may be used
            foreach (var dbProp in dbProps)
            {
                var columnName = dbProp.ColumnName;
                bulkCopy.ColumnMappings.Add(columnName, columnName);
            }

            return bulkCopy;
        }
    }
}