using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Pomelo.EntityFrameworkCore.Lolita.Delete
{
    /// <summary>
    /// Default Lolita Delete Executor
    /// </summary>
    public class DefaultLolitaDeleteExecutor : ILolitaDeleteExecutor
    {
        private static FieldInfo EntityTypesField = typeof(Model).GetTypeInfo().DeclaredFields.Single(x => x.Name == "_entityTypes");

        /// <summary>
        /// Create a new instance of <see cref="DefaultLolitaDeleteExecutor"/>
        /// </summary>
        /// <param name="currentDbContext"></param>
        /// <param name="sqlGenerationHelper"></param>
        /// <param name="dbSetFinder"></param>
        public DefaultLolitaDeleteExecutor(ICurrentDbContext currentDbContext, ISqlGenerationHelper sqlGenerationHelper, IDbSetFinder dbSetFinder)
        {
            _sqlGenerationHelper = sqlGenerationHelper;
            _dbSetFinder = dbSetFinder;
            _context = currentDbContext.Context;
        }

        private ISqlGenerationHelper _sqlGenerationHelper;
        private IDbSetFinder _dbSetFinder;
        private DbContext _context;

        /// <summary>
        /// Parse table name
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected virtual string ParseTableName(EntityType type)
        {
            string tableName;
            var anno = type.FindAnnotation("Relational:TableName");
            if (anno != null)
                tableName = anno.Value.ToString();
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
        /// Get table name
        /// </summary>
        /// <param name="et"></param>
        /// <returns></returns>
        protected virtual string GetTableName(EntityType et)
        {
            return _sqlGenerationHelper.DelimitIdentifier(ParseTableName(et));
        }

        /// <summary>
        /// Get full table name
        /// </summary>
        /// <param name="et"></param>
        /// <returns></returns>
        protected virtual string GetFullTableName(EntityType et)
        {
            string schema = null;

            // first, try to get schema from fluent API or data annotation
            IAnnotation anno = et.FindAnnotation("Relational:Schema");
            if (anno != null)
                schema = anno.Value.ToString();
            if (schema == null)
            {
                // otherwise, try to get schema from context default
                anno = _context.Model.FindAnnotation("Relational:DefaultSchema");
                if (anno != null)
                    schema = anno.Value.ToString();
            }
            // TODO: ideally, switch to `et.Relational().Schema`, covering all cases
            if (schema != null)
                return $"{_sqlGenerationHelper.DelimitIdentifier(schema)}.{_sqlGenerationHelper.DelimitIdentifier(ParseTableName(et))}";
            else
                return _sqlGenerationHelper.DelimitIdentifier(ParseTableName(et));
        }

        /// <summary>
        /// Generate sql
        /// </summary>
        /// <param name="lolita"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public virtual string GenerateSql<TEntity>(IQueryable<TEntity> lolita) where TEntity : class, new()
        {
            var sb = new StringBuilder("DELETE FROM ");
            var model = lolita.ElementType;
            var visitor = lolita.CompileQuery();

            var entities = (IDictionary<string, EntityType>)EntityTypesField.GetValue(_context.Model);
            var et = entities.Where(x => x.Value.ClrType == typeof(TEntity)).Single().Value;

            var table = GetTableName(et);
            var fullTable = GetFullTableName(et);
            sb.Append(fullTable)
                .AppendLine()
                .Append(ParseWhere(visitor, table))
                .Append(_sqlGenerationHelper.StatementTerminator);

            return sb.ToString();
        }

        /// <summary>
        /// Parse where
        /// </summary>
        /// <param name="visitor"></param>
        /// <param name="Table"></param>
        /// <returns></returns>
        protected virtual string ParseWhere(RelationalQueryModelVisitor visitor, string Table)
        {
            if (visitor == null || visitor.Queries.Count == 0)
                return "";
            var sql = visitor.Queries.First().ToString();
            var pos = sql.IndexOf("WHERE", StringComparison.Ordinal);
            if (pos < 0)
                return "";
            return sql.Substring(pos)
                .Replace(_sqlGenerationHelper.DelimitIdentifier(visitor.CurrentParameter.Name), Table);
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public virtual int Execute(DbContext db, string sql)
        {
            return db.Database.ExecuteSqlCommand(sql);
        }

        /// <summary>
        /// Execute async
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<int> ExecuteAsync(DbContext db, string sql, CancellationToken cancellationToken = default(CancellationToken))
        {
            return db.Database.ExecuteSqlCommandAsync(sql, cancellationToken);
        }
    }
}