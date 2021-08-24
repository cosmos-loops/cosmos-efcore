using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Cosmos.EntityFrameworkCore.Internals
{
    [Obsolete("将由 Cosmos.Extensions.Reflection.Enhance 取代")]
    internal static class StrongTypedCache<T>
    {
        public static readonly ConcurrentDictionary<PropertyInfo, Func<T, object>> PropertyValueGetters = new();

        public static readonly ConcurrentDictionary<PropertyInfo, Action<T, object>> PropertyValueSetters = new();
    }
}