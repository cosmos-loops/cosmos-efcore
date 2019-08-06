using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Query;

namespace Pomelo.EntityFrameworkCore.Lolita.Update
{
    /// <summary>
    /// Default Lolita Update Executor
    /// </summary>
    public class DefaultLolitaUpdateExecutor : ILolitaUpdateExecutor
    {
        /// <summary>
        /// Create a new instance of <see cref="DefaultLolitaUpdateExecutor"/>
        /// </summary>
        /// <param name="currentDbContext"></param>
        /// <param name="sqlGenerationHelper"></param>
        /// <param name="dbSetFinder"></param>
        public DefaultLolitaUpdateExecutor(ICurrentDbContext currentDbContext, ISqlGenerationHelper sqlGenerationHelper, IDbSetFinder dbSetFinder)
        {
            _sqlGenerationHelper = sqlGenerationHelper;
            _dbSetFinder = dbSetFinder;
            _context = currentDbContext.Context;
        }

        private ISqlGenerationHelper _sqlGenerationHelper;
        private IDbSetFinder _dbSetFinder;
        private DbContext _context;

        /// <summary>
        /// Generate sql
        /// </summary>
        /// <param name="lolita"></param>
        /// <param name="visitor"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public virtual string GenerateSql<TEntity>(LolitaSetting<TEntity> lolita, RelationalQueryModelVisitor visitor) where TEntity : class, new()
        {
            var sb = new StringBuilder("UPDATE ");
            sb.Append(lolita.FullTable)
                .AppendLine()
                .Append("SET ")
                .Append(string.Join($", {Environment.NewLine}    ", lolita.Operations))
                .AppendLine()
                .Append(ParseWhere(visitor, lolita.ShortTable))
                .Append(_sqlGenerationHelper.StatementTerminator);

            return sb.ToString();
        }

        /// <summary>
        /// Parse where
        /// </summary>
        /// <param name="visitor"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        protected virtual string ParseWhere(RelationalQueryModelVisitor visitor, string table)
        {
            if (visitor == null || visitor.Queries.Count == 0)
                return "";
            var sql = visitor.Queries.First().ToString();
            var pos = sql.IndexOf("WHERE", StringComparison.Ordinal);
            if (pos < 0)
                return "";
            return sql.Substring(pos)
                .Replace(_sqlGenerationHelper.DelimitIdentifier(visitor.CurrentParameter.Name), table);
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual int Execute(DbContext db, string sql, object[] param)
        {
            return db.Database.ExecuteSqlCommand(sql, param);
        }

        /// <summary>
        /// Execute async
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public Task<int> ExecuteAsync(DbContext db, string sql, CancellationToken cancellationToken = default(CancellationToken), params object[] param)
        {
            return db.Database.ExecuteSqlCommandAsync(sql, cancellationToken, param);
        }
    }
}