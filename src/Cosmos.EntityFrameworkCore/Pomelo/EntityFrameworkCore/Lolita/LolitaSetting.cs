using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Pomelo.EntityFrameworkCore.Lolita
{
    /// <summary>
    /// Lolita setting
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class LolitaSetting<TEntity> where TEntity : class
    {
        /// <summary>
        /// Query
        /// </summary>
        public IQueryable<TEntity> Query { get; set; }

        /// <summary>
        /// Full table
        /// </summary>
        public string FullTable { get; set; }

        /// <summary>
        /// Short table
        /// </summary>
        public string ShortTable { get; set; }

        /// <summary>
        /// operations
        /// </summary>
        public IList<string> Operations { get; set; } = new List<string>();

        /// <summary>
        /// Parameters
        /// </summary>
        public IList<object> Parameters { get; set; } = new List<object>();

        /// <summary>
        /// Get services
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public TService GetService<TService>() => Query.GetService<TService>();
    }
}