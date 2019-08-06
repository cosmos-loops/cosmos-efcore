using Cosmos.Domain.Core;

namespace Cosmos.EntityFrameworkCore.Map
{
    /// <summary>
    /// Entity map
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class EntityMap<TEntity> : MapBase<TEntity>, ISqliteEntityMap where TEntity : class, IEntity, new() { }
}