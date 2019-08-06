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
        /// <summary>
        /// Find a result by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual TEntity FindById(TKey id)
        {
            return Set.Find(id);
        }

        /// <summary>
        /// Find a collection of result by a set of id
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual List<TEntity> FindByIds(params TKey[] ids)
        {
            return Set.Where(t => ids.Contains(t.Id)).ToList();
        }

        /// <summary>
        /// Find a collection of result by a set of id
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual List<TEntity> FindByIds(IEnumerable<TKey> ids)
        {
            return Set.Where(t => ids.Contains(t.Id)).ToList();
        }

        /// <summary>
        /// Find a collection of result by given condition
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return Set.Where(predicate);
        }

        /// <summary>
        /// Find a paged collection of result by given condition
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate, int pageNumber, int pageSize)
        {
            return Set.Where(predicate).Skip((pageNumber - 1) + pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        /// Find first or default result
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual TEntity FindFirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return Set.FirstOrDefault(predicate);
        }

        /// <summary>
        /// Query paged results
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual IPage<TEntity> QueryPage(Expression<Func<TEntity, bool>> predicate, int pageNumber, int pageSize)
        {
            return Set.Where(predicate).GetPage(pageNumber, pageSize);
        }

        /// <summary>
        /// Find a result by id async
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task<TEntity> FindByIdAsync(TKey id, CancellationToken cancellationToken = default)
        {
            return Set.FindAsync(new object[] {id}, cancellationToken);
        }

        /// <summary>
        /// Find a collection of result by a set of id async
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual Task<List<TEntity>> FindByIdsAsync(params TKey[] ids)
        {
            return Set.Where(t => ids.Contains(t.Id)).ToListAsync();
        }

        /// <summary>
        /// Find a collection of result by a set of id async
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task<List<TEntity>> FindByIdsAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
        {
            return Set.Where(t => ids.Contains(t.Id)).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Find a paged collection of result by given condition async
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>> predicate, int pageNumber, int pageSize,
            CancellationToken cancellationToken = default)
        {
            return await Set.Where(predicate).Skip((pageNumber - 1) + pageSize).Take(pageSize).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Find first or default result async
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual Task<TEntity> FindFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Set.FirstOrDefaultAsync(predicate);
        }

        /// <summary>
        /// Query paged results async
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual Task<IPage<TEntity>> QueryPageAsync(Expression<Func<TEntity, bool>> predicate, int pageNumber, int pageSize)
        {
            return Task.FromResult(QueryPage(predicate, pageNumber, pageSize));
        }
    }
}