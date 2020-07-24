using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Pomelo.EntityFrameworkCore.Lolita.Update
{
    /// <summary>
    /// Default field parser
    /// </summary>
    public class DefaultFieldParser : IFieldParser
    {
        private static FieldInfo EntityTypesField = typeof(Model).GetTypeInfo().DeclaredFields.Single(x => x.Name == "_entityTypes");

        /// <summary>
        /// Create a new instance of <see cref="DefaultFieldParser"/>
        /// </summary>
        /// <param name="currentDbContext"></param>
        /// <param name="sqlGenerationHelper"></param>
        /// <param name="dbSetFinder"></param>
        public DefaultFieldParser(ICurrentDbContext currentDbContext, ISqlGenerationHelper sqlGenerationHelper, IDbSetFinder dbSetFinder)
        {
            _sqlGenerationHelper = sqlGenerationHelper;
            _dbSetFinder = dbSetFinder;
            _context = currentDbContext.Context;
        }

        private ISqlGenerationHelper _sqlGenerationHelper;
        private IDbSetFinder _dbSetFinder;
        private DbContext _context;

        /// <summary>
        /// Parse field
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public virtual string ParseField(SqlFieldInfo field)
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(field.Table))
                sb.Append(_sqlGenerationHelper.DelimitIdentifier(field.Table))
                   .Append(".");
            sb.Append(_sqlGenerationHelper.DelimitIdentifier(field.Column));
            return sb.ToString();
        }

        /// <summary>
        /// Parse full table
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public virtual string ParseFullTable(SqlFieldInfo field)
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(field.Database))
                sb.Append(_sqlGenerationHelper.DelimitIdentifier(field.Database))
                   .Append(".");
            if (!string.IsNullOrEmpty(field.Schema))
                sb.Append(_sqlGenerationHelper.DelimitIdentifier(field.Schema))
                   .Append(".");
            sb.Append(_sqlGenerationHelper.DelimitIdentifier(field.Table));
            return sb.ToString();
        }

        /// <summary>
        /// Parse short table
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public virtual string ParseShortTable(SqlFieldInfo field)
        {
            return _sqlGenerationHelper.DelimitIdentifier(field.Table);
        }

        /// <summary>
        /// Get table name
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected virtual string GetTableName(EntityType type)
        {
            string tableName;
            var anno = type.FindAnnotation("Relational:TableName");
            if (anno != null)
            {
                tableName = anno.Value.ToString();
            }
            else
            {
                var prop = _dbSetFinder.FindSets(_context).SingleOrDefault(y => y.ClrType == type.ClrType);
                if (!prop.Equals(default(DbSetProperty)))
                    tableName = prop.Name;
                else
                    tableName = type.ClrType.Name;
            }

            return tableName;
        }

        /// <summary>
        /// Get schema name
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected virtual string GetSchemaName(EntityType type)
        {
            string schema = null;

            // first, try to get schema from fluent API or data annotation
            IAnnotation anno = type.FindAnnotation("Relational:Schema");
            if (anno != null)
                schema = anno.Value.ToString();
            if (schema == null)
            {
                // otherwise, try to get schema from context default
                anno = _context.Model.FindAnnotation("Relational:DefaultSchema");
                if (anno != null)
                    schema = anno.Value.ToString();
            }
            // TODO: ideally, switch to `et.Relational().Schema`, to cover all cases at once

            return schema;
        }

        /// <summary>
        /// Visit field
        /// </summary>
        /// <param name="exp"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        public virtual SqlFieldInfo VisitField<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> exp)
        where TEntity : class, new()
        {
            var ret = new SqlFieldInfo();

            // Getting table name and schema name
            if (exp.Parameters.Count != 1)
            {
                throw new ArgumentException("Too many parameters in the expression.");
            }

            var param = exp.Parameters.Single();
            var entities = (IDictionary<string, EntityType>) EntityTypesField.GetValue(_context.Model);
            var et = entities.Single(x => x.Value.ClrType == typeof(TEntity)).Value;
            ret.Table = GetTableName(et);
            ret.Schema = GetSchemaName(et);

            // Getting field name
            if (!(exp.Body is MemberExpression body))
            {
                throw new NotSupportedException(exp.Body.GetType().Name);
            }

            var columnAttr = body.Member.GetCustomAttribute<ColumnAttribute>();
            if (columnAttr != null)
            {
                ret.Column = columnAttr.Name;
            }
            else
            {
                ret.Column = body.Member.Name;
            }

            return ret;
        }
    }
}