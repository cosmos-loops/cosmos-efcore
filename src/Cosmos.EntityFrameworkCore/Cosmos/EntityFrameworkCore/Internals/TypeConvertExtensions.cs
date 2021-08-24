using System;
using Cosmos.Exceptions;

namespace Cosmos.EntityFrameworkCore.Internals
{
    internal static class TypeConvertExtensions
    {
        public static object GetEnumValue(this Type typeOfEnum, int value)
        {
            return Try.Create(() => EnumsNET.Enums.ToObject(typeOfEnum, value))
                      .GetSafeValue((object)null);
        }
    }
}