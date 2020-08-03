namespace System.Reflection
{
    /// <summary>
    /// Cosmos type extensions
    /// </summary>
    public static class CosmosTypeExtensions
    {
        public static bool IsSqlSimpleType(this Type type)
        {
            var t = type;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                t = Nullable.GetUnderlyingType(type);

            return t!.IsPrimitive
                   || t == typeof(string)
                   || t == typeof(DateTime)
                   || t == typeof(DateTimeOffset)
                   || t == typeof(TimeSpan)
                   || t == typeof(Guid)
                   || t == typeof(byte[])
                   || t == typeof(char[]);
        }
    }
}