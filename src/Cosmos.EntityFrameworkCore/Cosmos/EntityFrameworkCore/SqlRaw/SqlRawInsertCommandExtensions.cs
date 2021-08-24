using System;
using System.Collections.Generic;
using System.Data;
using Cosmos.EntityFrameworkCore.Internals;
using Cosmos.Models;
using Microsoft.EntityFrameworkCore;

/*
 * Reference: Zack.EFCore.Batch
 *   url: https://github.com/yangzhongke/Zack.EFCore.Batch
 *   author: 杨中科
 */

namespace Cosmos.EntityFrameworkCore.SqlRaw
{
    internal static class SqlRawInsertCommandExtensions
    {
        /// <summary>
        /// Build DataTable for items
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbSet"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static DataTable BuildDataTable<TEntity>(this DbSet<TEntity> dbSet, IEnumerable<TEntity> items)
            where TEntity : class, IEntity
        {
            var databaseProperties = dbSet.EntityType.GetColumnInfoColl<TEntity>();

            var dataTable = new DataTable();
            foreach (var databaseProperty in databaseProperties)
            {
                var columnName = databaseProperty.ColumnName;
                var propertyType = databaseProperty.PropertyType;
                if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    dataTable.Columns.Add(columnName, propertyType.GetGenericArguments()[0]).AllowDBNull = true;
                else
                    dataTable.Columns.Add(columnName, propertyType);
            }

            foreach (var item in items)
            {
                var row = dataTable.NewRow();

                foreach (var databaseProperty in databaseProperties)
                    row[databaseProperty.ColumnName] = databaseProperty.Property.GetValue(item) ?? DBNull.Value;

                dataTable.Rows.Add(row);
            }

            return dataTable;
        }
    }
}