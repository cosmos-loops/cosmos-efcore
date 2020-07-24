namespace Cosmos.EntityFrameworkCore
{
    /// <summary>
    /// EfCore options
    /// </summary>
    public class EfCoreOptions
    {
        /// <summary>
        /// Connection name
        /// </summary>
        public string ConnectionName { get; set; }

        /// <summary>
        /// Connection string
        /// </summary>
        public string ConnectionString { get; set; }
    }
}