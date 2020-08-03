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
    public abstract partial class StoreBase<TContext, TEntity, TKey>
    {
        #region Find # in base IStore.QueryableStore

        /// <summary>
        /// Find first or default result
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return Set.Where(predicate).ToList();
        }

        /// <summary>
        /// Find first or default result async
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await Set.Where(predicate).ToListAsync(cancellationToken);
        }

        #endregion

        #region Find by id # in base IStore.QueryableStore

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
        /// Find a result by id async
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task<TEntity> FindByIdAsync(TKey id, CancellationToken cancellationToken = default)
        {
            return Set.FindAsync(new object[] {id}, cancellationToken).AsTask();
        }

        #endregion

        #region Find by ids # in IStore.QueryableStore.Query

        /// <summary>
        /// Find a collection of result by a set of id
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> FindByIds(params TKey[] ids)
        {
            return Set.Where(t => ids.Contains(t.Id)).ToList();
        }

        /// <summary>
        /// Find a collection of result by a set of id
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> FindByIds(IEnumerable<TKey> ids)
        {
            return Set.Where(t => ids.Contains(t.Id)).ToList();
        }

        /// <summary>
        /// Find a collection of result by a set of id async
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<TEntity>> FindByIdsAsync(params TKey[] ids)
        {
            return await Set.Where(t => ids.Contains(t.Id)).ToListAsync();
        }

        /// <summary>
        /// Find a collection of result by a set of id async
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<TEntity>> FindByIdsAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
        {
            return await Set.Where(t => ids.Contains(t.Id)).ToListAsync(cancellationToken);
        }

        #endregion

        #region Find First # in base IStore.QueryableStore

        /// <summary>
        /// Find first  result by given condition
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual TEntity FindFirst(Expression<Func<TEntity, bool>> predicate)
        {
            return Set.First(predicate);
        }

        /// <summary>
        /// Find first  result by given condition async
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task<TEntity> FindFirstAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return Set.FirstAsync(predicate, cancellationToken);
        }

        #endregion

        #region Find First Or Default # in base IStore.QueryableStore

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
        /// Find first or default result async
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task<TEntity> FindFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return Set.FirstOrDefaultAsync(predicate, cancellationToken);
        }

        #endregion

        #region Find Single # in base IStore.QueryableStore

        /// <summary>
        /// Find single result by given condition
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual TEntity FindSingle(Expression<Func<TEntity, bool>> predicate)
        {
            return Set.Single(predicate);
        }

        /// <summary>
        /// Find single result by given condition async
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task<TEntity> FindSingleAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return Set.SingleAsync(predicate, cancellationToken);
        }

        #endregion

        #region Find Single Or Default # in base IStore.QueryableStore

        /// <summary>
        /// Find single or default result
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual TEntity FindSingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return Set.SingleOrDefault(predicate);
        }

        /// <summary>
        /// Find single or default result async
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task<TEntity> FindSingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return Set.SingleOrDefaultAsync(predicate, cancellationToken);
        }

        #endregion

        #region Get One # in base IStore.QueryableStore

        /// <summary>
        /// Get one or null
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual TEntity GetOne(TKey id) => FindById(id);

        /// <summary>
        /// Get one or null...
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual TEntity GetOne(Expression<Func<TEntity, bool>> predicate) => FindFirstOrDefault(predicate);

        /// <summary>
        /// Get one or null
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task<TEntity> GetOneAsync(TKey id, CancellationToken cancellationToken = default) => FindByIdAsync(id, cancellationToken);

        /// <summary>
        /// Get one or null...
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
            => FindFirstOrDefaultAsync(predicate, cancellationToken);

        #endregion

        #region Get page # in base IStore.QueryableStore

        /// <summary>
        /// Query pageable results by given condition
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual IPage<TEntity> GetPage(Expression<Func<TEntity, bool>> predicate, int pageNumber, int pageSize)
        {
            return Set.Where(predicate).GetPage(pageNumber, pageSize);
        }

        /// <summary>
        /// Query pageable results by given condition async
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task<IPage<TEntity>> GetPageAsync(Expression<Func<TEntity, bool>> predicate, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Set.Where(predicate).GetPage(pageNumber, pageSize));
        }

        #endregion
    }
}