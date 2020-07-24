using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.Lolita;

namespace Cosmos.EntityFrameworkCore.Store
{
    public abstract partial class StoreBase<TContext, TEntity, TKey>
    {
        /// <summary>
        /// 执行非安全更新操作，该操作无视工作单元，直接生效于数据库。
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="updateFunc"></param>
        /// <param name="updateFunc2"></param>
        public virtual void ExecuteUnsafeUpdate(
            Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, LolitaSetting<TEntity>> updateFunc,
            params Func<LolitaSetting<TEntity>, LolitaSetting<TEntity>>[] updateFunc2)
        {
            var entry = Set.Where(predicate);
            var lolitaEntry = updateFunc(entry);
            if (updateFunc2 != null && updateFunc2.Any())
                lolitaEntry = updateFunc2.Aggregate(lolitaEntry, (current, func) => func(current));
            lolitaEntry.Update();
        }

        /// <summary>
        /// 执行非安全更新操作，该操作无视工作单元，直接生效于数据库。
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="updateFunc"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="updateFunc2"></param>
        /// <returns></returns>
        public virtual Task ExecuteUnsafeUpdateAsync(
            Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, LolitaSetting<TEntity>> updateFunc,
            CancellationToken cancellationToken = default,
            params Func<LolitaSetting<TEntity>, LolitaSetting<TEntity>>[] updateFunc2)
        {
            var entry = Set.Where(predicate);
            var lolitaEntry = updateFunc(entry);
            if (updateFunc2 != null && updateFunc2.Any())
                lolitaEntry = updateFunc2.Aggregate(lolitaEntry, (current, func) => func(current));
            return lolitaEntry.UpdateAsync(cancellationToken);
        }

        /// <summary>
        /// 执行非安全删除操作，该操作无视工作单元，直接生效于数据库。
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="predicate2"></param>
        public virtual void ExecuteUnsafeDelete(
            Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, bool>>[] predicate2)
        {
            var entry = Set.Where(predicate);
            if (predicate2 != null && predicate2.Any())
                entry = predicate2.Aggregate(entry, (current, func) => current.Where(func));
            entry.Delete();
        }

        /// <summary>
        /// 执行非安全删除操作，该操作无视工作单元，直接生效于数据库。
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="predicate2"></param>
        public virtual Task ExecuteUnsafeDeleteAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, bool>>[] predicate2)
        {
            var entry = Set.Where(predicate);
            if (predicate2 != null && predicate2.Any())
                entry = predicate2.Aggregate(entry, (current, func) => current.Where(func));
            return entry.DeleteAsync(cancellationToken);
        }
    }
}