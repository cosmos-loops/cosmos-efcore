using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Cosmos.EntityFrameworkCore.SqlRaw;
using Cosmos.Models;
using Microsoft.EntityFrameworkCore;

namespace Cosmos.EntityFrameworkCore
{
    public class FluentUpdateBuilder<TEntity> where TEntity : class, IEntity, new()
    {
        private readonly List<SqlRawUpdateCommandGenerator<TEntity>.EntitySettingEntry> _entryColl = new();

        private readonly DbContext _dbContext;
        private readonly DbSet<TEntity> _dbSet;

        private int? _skip;
        private int? _take;

        public FluentUpdateBuilder(DbContext dbContext, DbSet<TEntity> dbSet)
        {
            _dbContext = dbContext;
            _dbSet = dbSet;
        }

        /// <summary>
        /// name is the expression of property's name, and value is the expression of the value
        /// </summary>
        /// <param name="name">something like: b=>b.Age</param>
        /// <param name="value">something like: b=>b.Age+1</param>
        /// <returns></returns>
        public FluentUpdateBuilder<TEntity> Set<TP>(Expression<Func<TEntity, TP>> name, Expression<Func<TEntity, TP>> value)
        {
            if (name.Body is not MemberExpression propExpression)
                throw new InvalidOperationException("name.Body is not a MemberExpression instance.");
            _entryColl.Add(new SqlRawUpdateCommandGenerator<TEntity>.EntitySettingEntry { Name = name, Value = value, PropertyType = typeof(TP), PropertyName = propExpression.Member.Name });
            return this;
        }

        private Expression<Func<TEntity, bool>> _predicate;

        public FluentUpdateBuilder<TEntity> Where(Expression<Func<TEntity, bool>> predicate = null)
        {
            _predicate = predicate;
            return this;
        }

        public FluentUpdateBuilder<TEntity> Skip(int skipCount)
        {
            _skip = skipCount;
            return this;
        }

        public FluentUpdateBuilder<TEntity> Take(int takeCount)
        {
            _take = takeCount;
            return this;
        }

        public int Execute(bool ignoreQueryFilters = false)
        {
            var sql = SqlRawUpdateCommandGenerator<TEntity>
                .Generate(_dbContext, _dbSet, _entryColl, _predicate, ignoreQueryFilters, _skip, _take, out var parameters);
            return SqlRawWorker.Execute(_dbContext, sql, parameters);
        }

        public Task<int> ExecuteAsync(bool ignoreQueryFilters = false, CancellationToken cancellationToken = default)
        {
            var sql = SqlRawUpdateCommandGenerator<TEntity>
                .Generate(_dbContext, _dbSet, _entryColl, _predicate, ignoreQueryFilters, _skip, _take, out var parameters);
            return SqlRawWorker.ExecuteAsync(_dbContext, sql, parameters, cancellationToken);
        }
    }
}