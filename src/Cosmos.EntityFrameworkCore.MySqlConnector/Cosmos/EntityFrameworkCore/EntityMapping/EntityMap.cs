using Cosmos.Models;

namespace Cosmos.EntityFrameworkCore.EntityMapping
{
    /// <summary>
    /// Entity map
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class EntityMap<TEntity> : MapBase<TEntity>, IMySqlEntityMap where TEntity : class, IEntity, new() { }
}