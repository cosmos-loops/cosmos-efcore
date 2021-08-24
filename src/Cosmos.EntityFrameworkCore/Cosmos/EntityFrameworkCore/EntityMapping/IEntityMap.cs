using Microsoft.EntityFrameworkCore;

namespace Cosmos.EntityFrameworkCore.EntityMapping
{
    /// <summary>
    /// Interface for Entity Map
    /// </summary>
    public interface IEntityMap
    {
        /// <summary>
        /// Map
        /// </summary>
        /// <param name="builder"></param>
        void Map(ModelBuilder builder);
    }
}