using System;
using System.Linq;
using System.Linq.Expressions;

namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Execute for include
    /// </summary>
    public static class IncludeExtensions
    {
        /// <summary>
        /// Include if need...
        /// </summary>
        /// <param name="source"></param>
        /// <param name="condition"></param>
        /// <param name="property"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <returns></returns>
        public static IQueryable<TEntity> IncludeIfNeed<TEntity, TProperty>(
            this IQueryable<TEntity> source,
            bool condition,
            Expression<Func<TEntity, TProperty>> property) where TEntity : class
        {
            return condition
                ? source.Include(property)
                : source;
        }

        /// <summary>
        /// Include if need...
        /// </summary>
        /// <param name="source"></param>
        /// <param name="condition"></param>
        /// <param name="property"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <returns></returns>
        public static IQueryable<TEntity> IncludeIfNeed<TEntity, TProperty>(
            this IQueryable<TEntity> source,
            Func<bool> condition,
            Expression<Func<TEntity, TProperty>> property) where TEntity : class
        {
            return condition()
                ? source.Include(property)
                : source;
        }
    }
}