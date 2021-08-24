using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Cosmos.Data.Sx.SqlBulkCopy;
using Cosmos.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;

namespace Cosmos.EntityFrameworkCore.SqlRaw
{
    public static class NpgSqlRawInsertExtensions
    {
        public static void BulkInsert<TEntity>(this DbContext dbCtx,
            IEnumerable<TEntity> items) where TEntity : class, IEntity
        {
            var dbSet = dbCtx.Set<TEntity>();
            var entityType = dbSet.EntityType;
            var sqlGenHelper = dbCtx.GetService<ISqlGenerationHelper>();

            using (var bulkCopy = CreateSqlBulkCopy(dbCtx))
            {
                bulkCopy.DestinationTableName = TouchTableName(entityType, sqlGenHelper);

                var table = TouchDataTable(dbSet, items);
                bulkCopy.WriteToServer(table);
            }
        }

        public static async Task BulkInsertAsync<TEntity>(this DbContext dbCtx,
            IEnumerable<TEntity> items, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            var dbSet = dbCtx.Set<TEntity>();
            var entityType = dbSet.EntityType;
            var sqlGenHelper = dbCtx.GetService<ISqlGenerationHelper>();

            using (var bulkCopy = CreateSqlBulkCopy(dbCtx))
            {
                bulkCopy.DestinationTableName = TouchTableName(entityType, sqlGenHelper);

                var table = TouchDataTable(dbSet, items);
                await bulkCopy.WriteToServerAsync(table, cancellationToken);
            }
        }

        private static NpgSqlBulkCopy CreateSqlBulkCopy(DbContext dbCtx)
        {
            return new NpgSqlBulkCopy((NpgsqlConnection)dbCtx.Database.GetDbConnection());
        }

        private static DataTable TouchDataTable<TEntity>(DbSet<TEntity> dbSet, IEnumerable<TEntity> items) where TEntity : class, IEntity
        {
            return dbSet.BuildDataTable(items);
        }

        private static string TouchTableName(IEntityType entityType, ISqlGenerationHelper sqlGenHelper)
        {
            //"COPY myschema.t_books" doesn't work, we should use "myschema"."t_books" instead
            var schemaName = entityType.GetSchema();
            var tableName = entityType.GetTableName();

            if (string.IsNullOrWhiteSpace(schemaName))
                return sqlGenHelper.DelimitIdentifier(tableName);

            return sqlGenHelper.DelimitIdentifier(schemaName) + "." + sqlGenHelper.DelimitIdentifier(tableName);
        }
    }
}