using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;

/*
 * Reference: Zack.EFCore.Batch
 *   url: https://github.com/yangzhongke/Zack.EFCore.Batch
 *   author: 杨中科
 */

namespace Cosmos.EntityFrameworkCore.Internals
{
    internal class ColumnInfo
    {
        public IProperty Metadata { get; set; }

        public string ColumnName { get; set; }

        public string ColumnType { get; set; }

        public PropertyInfo Property { get; set; }

        public string PropertyName { get; set; }

        public Type PropertyType { get; set; }
    }
}