using Microsoft.EntityFrameworkCore;

namespace Cosmos.EntityFrameworkCore.Map
{
    public interface IEntityMap
    {
        void Map(ModelBuilder builder);
    }
}