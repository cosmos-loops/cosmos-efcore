using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Cosmos.EntityFrameworkCore.Map
{
    public static class MapBaseExtension
    {
        private static readonly ConcurrentDictionary<(Type, int), bool> MatchedCached = new ConcurrentDictionary<(Type, int), bool>();

        public static bool IsMatchedEntityMappingRule(this Type type, List<Type> bodyTypes)
        {
            if (type == null)
                return false;

            if (bodyTypes == null || !bodyTypes.Any())
                return false;

            if (CheckCache(type, bodyTypes, out var ret))
                return ret;

            if (!typeof(IEntityMap).IsAssignableFrom(type))
                return CacheAndReturn(type, bodyTypes, false);

            var bodyType = Types.GetRawTypeFromGenericClass(type, typeof(MapBase<>));

            if (bodyType == null)
                return CacheAndReturn(type, bodyTypes, false);

            return CacheAndReturn(type, bodyTypes, bodyTypes.Contains(bodyType));
        }

        private static bool CheckCache(Type type, IEnumerable<Type> bodyTypes, out bool ret)
        {
            var hashCode = bodyTypes.GetHashCode();
            return MatchedCached.TryGetValue((type, hashCode), out ret);
        }

        private static bool CacheAndReturn(Type type, IEnumerable<Type> bodyTypes, bool result)
        {
            var hashCode = bodyTypes.GetHashCode();
            MatchedCached.TryAdd((type, hashCode), result);
            return result;
        }
    }
}