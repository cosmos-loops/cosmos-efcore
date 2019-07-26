using System;
using System.Linq;
using System.Linq.Expressions;

namespace Microsoft.EntityFrameworkCore
{
    public static class IncludeExtensions
    {
        public static IQueryable<TEntity> IncludeIfNeed<TEntity, TProperty>(
            this IQueryable<TEntity> source,
            bool condition,
            Expression<Func<TEntity, TProperty>> property) where TEntity : class
        {
            return condition
                ? source.Include(property)
                : source;
        }

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