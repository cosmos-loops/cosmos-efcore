using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Cosmos.EntityFrameworkCore.Internals;
using Cosmos.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;

/*
 * Reference: Zack.EFCore.Batch
 *   url: https://github.com/yangzhongke/Zack.EFCore.Batch
 *   author: 杨中科
 */

namespace Cosmos.EntityFrameworkCore.SqlRaw
{
    internal static class SqlRawUpdateCommandGenerator<TEntity>
        where TEntity : class, IEntity
    {
        public static string Generate(
            DbContext dbContext,
            DbSet<TEntity> dbSet,
            List<EntitySettingEntry> entryColl,
            Expression<Func<TEntity, bool>> predicate,
            bool ignoreQueryFilters, int? skip, int? take,
            out IReadOnlyDictionary<string, object> parameters)

        {
            if (entryColl.Count <= 0)
                throw new InvalidOperationException("At least a Set() should be used.");

            var sqlGenHelper = dbContext.GetService<ISqlGenerationHelper>();

            //every pair of name=value are converted into two columns of Select,
            //for example, Set(b=>b.Age,b=>b.Age+3).Set(b=>b.Name,b=>"tom") is converted into
            //Select(b=>new{b.Age,F1=b.Age+3,b.Name,F2="tom"})     
            var parameter = Expression.Parameter(typeof(TEntity), "e");
            var initializers = new Expression[entryColl.Count * 2];
            for (var i = 0; i < entryColl.Count; i++)
            {
                var setter = entryColl[i];
                var propertyType = typeof(object);
                initializers[i * 2] = Expression.Convert(Expression.Invoke(setter.Name, parameter), propertyType);
                initializers[i * 2 + 1] = Expression.Convert(Expression.Invoke(setter.Value, parameter), propertyType);
            }

            // from https://stackoverflow.com/questions/47513122/entity-framework-core-dynamically-build-select-list-with-navigational-propertie
            var newArrayExpression = Expression.NewArrayInit(typeof(object), initializers);
            var selectExpression = Expression.Lambda<Func<TEntity, object>>(newArrayExpression, parameter);

            //IQueryable <TEntity> queryable = this.dbContext.Set<TEntity>();
            var queryable = Filter(dbSet, predicate, skip, take);

            var parsingRet = queryable.Select(selectExpression).Parse(dbContext, ignoreQueryFilters);
            var tableName = sqlGenHelper.DelimitIdentifier(parsingRet.TableName, parsingRet.Schema);

            // ---> UPDATE {TABLE_NAME} SET
            var sqlBuilder = new StringBuilder().UPDATE(tableName);

            var columns = parsingRet.ProjectionSql.ToArray();
            if (columns.Length % 2 != 0)
                throw new InvalidOperationException("The count of columns should be even.");

            // ---> Age = Age+3, Name = "tom"
            //combine every two adjacent columns into an assignment expression,
            //for example, select Age,Age+3,Name,"tom" is converted into
            //Age=Age+3,Name="tom"
            var keyColConverter = new IndexedKeyConverter();
            var valueColConverter = new IndexedValueConverter(dbContext, entryColl);
            Joiners.Joiner
                   .On(", ")
                   .WithKeyValueSeparator(" = ")
                   .AppendTo(sqlBuilder, columns, keyColConverter, valueColConverter);

            // ---> WHERE
            sqlBuilder.NEWLINE();
            if (parsingRet.FullSql.Contains("join", StringComparison.OrdinalIgnoreCase))
            {
                var aliasSeparator = parsingRet.QuerySqlGenerator.LocalAliasSeparator;
                sqlBuilder.WHERE(SqlRawHelper.ToSubQuerySqlForWhereSegment(queryable, dbContext, aliasSeparator));
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(parsingRet.PredicateSql))
                {
                    sqlBuilder.WHERE(parsingRet.PredicateSql);
                }
            }

            parameters = parsingRet.Parameters;

            return sqlBuilder.ToString();
        }

        private static IQueryable<TEntity> Filter(
            IQueryable<TEntity> queryable,
            Expression<Func<TEntity, bool>> predicate, int? skip, int? take)
        {
            if (predicate is not null)
                queryable = queryable.Where(predicate);

            if (skip is not null)
                queryable = queryable.Skip((int)skip);

            if (take is not null)
                queryable = queryable.Take((int)take);

            return queryable;
        }


        public class EntitySettingEntry
        {
            public LambdaExpression Name { get; set; }
            public LambdaExpression Value { get; set; }
            public Type PropertyType { get; set; }
            public string PropertyName { get; set; }
        }

        public class IndexedKeyConverter : IIndexedTypeConverter<string, string>
        {
            public string To(string from, int index)
            {
                return from;
            }
        }

        public class IndexedValueConverter : IIndexedTypeConverter<string, string>
        {
            private readonly IList<EntitySettingEntry> _entryColl;
            private readonly DbContext _dbContext;
            private readonly IEntityType _entityType;

            public IndexedValueConverter(DbContext dbContext, IList<EntitySettingEntry> entryColl)
            {
                _entryColl = entryColl;
                _dbContext = dbContext;
                _entityType = dbContext.Model.FindEntityType(typeof(TEntity));
            }

            public string To(string from, int index)
            {
                var columnValue = from;

                var settingEntry = _entryColl[(index - 1) / 2];
                var property = _entityType.GetProperty(settingEntry.PropertyName);
                var valueConverter = property.GetValueConverter();

                if (valueConverter is not null && settingEntry.PropertyType.IsEnum)
                {
                    if (settingEntry.Value.Body is not ConstantExpression)
                        throw new NotSupportedException("Only assignment of constant values to enumerated types is supported currently.");

                    //when expression is put in Select(u=>u.Status), it will not be converted by converter,
                    //so I need convert it manually.
                    var enumValue = settingEntry.PropertyType.GetEnumValue(Convert.ToInt32(columnValue));
                    var convertedVal = valueConverter.ConvertToProvider(enumValue);

                    //single quote string const
                    columnValue = _dbContext.GetService<IRelationalTypeMappingSource>()
                                            .FindMapping(property)
                                            .GenerateProviderValueSqlLiteral(convertedVal);
                }

                return columnValue;
            }
        }
    }
}