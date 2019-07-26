using Cosmos.Domain.Core;
using Cosmos.Domain.EntityDescriptors;

namespace Cosmos.EntityFrameworkCore.Map
{
    public abstract class VersionableRootMapBase<TEntity> : MapBase<TEntity>
        where TEntity : class, IEntity, IVersionable, new() { }
}