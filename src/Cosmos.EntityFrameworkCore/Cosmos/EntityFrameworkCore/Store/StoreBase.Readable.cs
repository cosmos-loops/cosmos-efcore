using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using DotNetCore.Collections.Paginable;
using Microsoft.EntityFrameworkCore;

namespace Cosmos.EntityFrameworkCore.Store
{
    public abstract partial class StoreBase<TEntity, TKey>
    {
        public virtual TEntity FindById(TKey id)
        {
            return Set.Find(id);
        }

        public virtual List<TEntity> FindByIds(params TKey[] ids)
        {
            return Set.Where(t => ids.Contains(t.Id)).ToList();
        }

        public virtual List<TEntity> FindByIds(IEnumerable<TKey> ids)
        {
            return Set.Where(t => ids.Contains(t.Id)).ToList();
        }

        public virtual IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return Set.Where(predicate);
        }

        public virtual IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate, int pageNumber, int pageSize)
        {
            return Set.Where(predicate).Skip((pageNumber - 1) + pageSize).Take(pageSize).ToList();
        }

        public virtual TEntity FindFirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return Set.FirstOrDefault(predicate);
        }

        public virtual IPage<TEntity> QueryPage(Expression<Func<TEntity, bool>> predicate, int pageNumber, int pageSize)
        {
            return Set.Where(predicate).GetPage(pageNumber, pageSize);
        }

        public virtual Task<TEntity> FindByIdAsync(TKey id, CancellationToken cancellationToken = default)
        {
            return Set.FindAsync(new object[] { id }, cancellationToken);
        }

        public virtual Task<List<TEntity>> FindByIdsAsync(params TKey[] ids)
        {
            return Set.Where(t => ids.Contains(t.Id)).ToListAsync();
        }

        public virtual Task<List<TEntity>> FindByIdsAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
        {
            return Set.Where(t => ids.Contains(t.Id)).ToListAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>> predicate, int pageNumber, int pageSize,
            CancellationToken cancellationToken = default)
        {
            return await Set.Where(predicate).Skip((pageNumber - 1) + pageSize).Take(pageSize).ToListAsync(cancellationToken);
        }

        public virtual Task<TEntity> FindFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Set.FirstOrDefaultAsync(predicate);
        }

        public virtual Task<IPage<TEntity>> QueryPageAsync(Expression<Func<TEntity, bool>> predicate, int pageNumber, int pageSize)
        {
            return Task.FromResult(QueryPage(predicate, pageNumber, pageSize));
        }
    }
}