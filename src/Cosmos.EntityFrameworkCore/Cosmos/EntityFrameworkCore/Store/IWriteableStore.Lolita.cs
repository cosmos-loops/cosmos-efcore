using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Cosmos.Domain.Core;
using Cosmos.Validations.Parameters;
using Pomelo.EntityFrameworkCore.Lolita;

namespace Cosmos.EntityFrameworkCore.Store
{
    public interface ILolitaWriteableStore<TEntity, in TKey> where TEntity : class, IEntity<TKey>, new()
    {
        void ExecuteUnsafeUpdate(
            [NotNull] Expression<Func<TEntity, bool>> predicate,
            [NotNull] Func<IQueryable<TEntity>, LolitaSetting<TEntity>> updateFunc,
            params Func<LolitaSetting<TEntity>, LolitaSetting<TEntity>>[] updateFunc2);

        Task ExecuteUnsafeUpdateAsync(
            [NotNull] Expression<Func<TEntity, bool>> predicate,
            [NotNull] Func<IQueryable<TEntity>, LolitaSetting<TEntity>> updateFunc,
            CancellationToken cancellationToken = default,
            params Func<LolitaSetting<TEntity>, LolitaSetting<TEntity>>[] updateFunc2);

        void ExecuteUnsafeDelete(
            [NotNull] Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, bool>>[] predicate2);

        Task ExecuteUnsafeDeleteAsync(
            [NotNull] Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, bool>>[] predicate2);
    }
}