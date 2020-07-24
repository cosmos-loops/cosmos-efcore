using Microsoft.EntityFrameworkCore;

namespace Pomelo.EntityFrameworkCore.Lolita
{
    /// <summary>
    /// Lolita valuing
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    public class LolitaValuing<TEntity, TProperty>
    where TEntity : class, new()
    {
        /// <summary>
        /// Inner setting
        /// </summary>
        public LolitaSetting<TEntity> Inner { get; set; }

        /// <summary>
        /// Get servce
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public TService GetService<TService>() => Inner.Query.GetService<TService>();

        /// <summary>
        /// Gets or sets current field
        /// </summary>
        public string CurrentField { get; set; }
    }
}