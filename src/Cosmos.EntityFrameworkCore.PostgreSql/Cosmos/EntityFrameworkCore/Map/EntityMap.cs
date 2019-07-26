using Cosmos.Domain.Core;

namespace Cosmos.EntityFrameworkCore.Map
{
    public abstract class EntityMap<TEntity> : MapBase<TEntity>, IPostgreSqlEntityMap where TEntity : class, IEntity, new() { }
}