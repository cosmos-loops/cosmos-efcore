using Cosmos.Domain.Core;

namespace Cosmos.EntityFrameworkCore.Map
{
    public abstract class EntityMap<TEntity> : MapBase<TEntity>, ISqliteEntityMap where TEntity : class, IEntity, new() { }
}