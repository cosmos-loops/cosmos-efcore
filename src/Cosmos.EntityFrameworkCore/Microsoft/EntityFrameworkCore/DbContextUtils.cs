using System.Reflection;

namespace Microsoft.EntityFrameworkCore
{
    public static partial class DbContextUtils
    {
        private static BindingFlags _bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;

        private static TResult GetValue<TResult>(this FieldInfo field, object obj)
        {
            return (TResult) field.GetValue(obj);
        }
    }
}