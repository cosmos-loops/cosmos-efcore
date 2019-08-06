namespace Pomelo.EntityFrameworkCore.Lolita.Update
{
    /// <summary>
    /// Sql field info
    /// </summary>
    public class SqlFieldInfo
    {
        /// <summary>
        /// Database
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// Schema
        /// </summary>
        public string Schema { get; set; }

        /// <summary>
        /// Table
        /// </summary>
        public string Table { get; set; }

        /// <summary>
        /// Column
        /// </summary>
        public string Column { get; set; }
    }
}