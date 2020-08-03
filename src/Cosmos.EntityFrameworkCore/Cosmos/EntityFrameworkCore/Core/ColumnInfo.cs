using System.Reflection;

namespace Cosmos.EntityFrameworkCore.Core
{
    /// <summary>
    /// Column information
    /// </summary>
    internal class ColumnInfo
    {
        /// <summary>
        /// Column name
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Order
        /// </summary>
        public int Order { get; set; }
        
        /// <summary>
        /// Binding to
        /// </summary>
        public PropertyInfo Property { get; set; }
    }
}