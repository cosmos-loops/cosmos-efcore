using Cosmos.Domain.Core;
using Cosmos.Domain.EntityDescriptors;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cosmos.EntityFrameworkCore.Map
{
    public abstract class AggregateRootMap<TEntity> : VersionableRootMapBase<TEntity>, IMySqlEntityMap
        where TEntity : class, IEntity, IVersionable, new()
    {
        /// <summary>
        /// 映射乐观离线锁
        /// </summary>
        protected override void MapVersion(EntityTypeBuilder<TEntity> builder)
        {
            builder.Property(t => t.Version).IsConcurrencyToken();
        }
    }
}