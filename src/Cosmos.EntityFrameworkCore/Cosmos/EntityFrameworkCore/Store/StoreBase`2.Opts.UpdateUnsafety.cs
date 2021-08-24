namespace Cosmos.EntityFrameworkCore.Store
{
    public abstract partial class StoreBase<TContext, TEntity, TKey>
    {
        public virtual FluentUpdateBuilder<TEntity> UnsafeUpdate()
        {
            return new FluentUpdateBuilder<TEntity>(RawTypedContext, Set);
        }
    }
}