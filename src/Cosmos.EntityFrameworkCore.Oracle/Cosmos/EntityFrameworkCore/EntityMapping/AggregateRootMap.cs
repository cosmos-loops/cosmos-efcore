using Cosmos.Models;
using Cosmos.Models.Descriptors.EntityDescriptors;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cosmos.EntityFrameworkCore.EntityMapping
{
    /// <summary>
    /// Mapping config for AggregateRoot
    /// <br />
    /// 聚合根映射配置
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    public abstract class AggregateRootMap<TEntity> : MapBase<TEntity>, IOracleEntityMap
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