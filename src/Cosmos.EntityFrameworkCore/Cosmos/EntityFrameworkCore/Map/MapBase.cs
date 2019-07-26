using System;
using Cosmos.Domain.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cosmos.EntityFrameworkCore.Map
{
    public abstract class MapBase<TEntity> : IEntityMap where TEntity : class, IEntity, new()
    {
        /// <summary>
        /// 模型生成器
        /// </summary>
        protected ModelBuilder ModelBuilder { get; private set; }

        /// <summary>
        /// 映射配置
        /// </summary>
        /// <param name="modelBuilder">模型生成器</param>
        public void Map(ModelBuilder modelBuilder)
        {
            ModelBuilder = modelBuilder;

            var builder = ModelBuilder.Entity<TEntity>();

            MapTable(builder);
            MapVersion(builder);
            MapProperties(builder);
            MapAssociations(builder);

            //config global data filtering strategy
            HasQueryFilter(builder);
        }

        /// <summary>
        /// 映射表
        /// </summary>
        protected virtual void MapTable(EntityTypeBuilder<TEntity> builder) { }

        /// <summary>
        /// 映射乐观离线锁
        /// </summary>
        protected virtual void MapVersion(EntityTypeBuilder<TEntity> builder) { }

        /// <summary>
        /// 映射属性
        /// </summary>
        protected virtual void MapProperties(EntityTypeBuilder<TEntity> builder) { }

        /// <summary>
        /// 映射导航属性
        /// </summary>
        protected virtual void MapAssociations(EntityTypeBuilder<TEntity> builder) { }

        protected virtual void HasQueryFilter(EntityTypeBuilder<TEntity> builder) { }
    }
}