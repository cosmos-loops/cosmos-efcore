using Cosmos.Domain.Core;
using Cosmos.Domain.EntityDescriptors;

namespace Cosmos.EntityFrameworkCore.Map
{
    /// <summary>
    /// Versionable Root Map Base
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class VersionableRootMapBase<TEntity> : MapBase<TEntity>
    where TEntity : class, IEntity, IVersionable, new() { }
}