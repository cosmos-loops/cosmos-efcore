using System;
using System.Linq;
using System.Linq.Expressions;
using Pomelo.EntityFrameworkCore.Lolita;
using Pomelo.EntityFrameworkCore.Lolita.Update;

/*
 * reference to:
 *     PomeloFoundation/Lolita
 *     Author: Yuko & PomeloFoundation
 *     URL: https://github.com/PomeloFoundation/Lolita
 *     MIT
 */

namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Extensions for <see cref="DbSet{T}"/>
    /// </summary>
    public static class DbSetExtensions
    {
        /// <summary>
        /// Set field
        /// </summary>
        /// <param name="self"></param>
        /// <param name="setValueExpression"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static LolitaValuing<TEntity, TProperty> SetField<TEntity, TProperty>(this IQueryable<TEntity> self, Expression<Func<TEntity, TProperty>> setValueExpression)
        where TEntity : class, new()
        {
            if (setValueExpression == null)
                throw new ArgumentNullException("setValueExpression");

            var factory = self.GetService<IFieldParser>();
            var sqlField = factory.VisitField(setValueExpression);

            var inner = new LolitaSetting<TEntity> {Query = self, FullTable = factory.ParseFullTable(sqlField), ShortTable = factory.ParseShortTable(sqlField)};
            return new LolitaValuing<TEntity, TProperty> {Inner = inner, CurrentField = factory.ParseField(sqlField)};
        }

        /// <summary>
        /// Set field
        /// </summary>
        /// <param name="self"></param>
        /// <param name="setValueExpression"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static LolitaValuing<TEntity, TProperty> SetField<TEntity, TProperty>(this LolitaSetting<TEntity> self, Expression<Func<TEntity, TProperty>> setValueExpression)
        where TEntity : class, new()
        {
            if (setValueExpression == null)
                throw new ArgumentNullException("setValueExpression");

            var factory = self.GetService<IFieldParser>();
            var sqlField = factory.VisitField(setValueExpression);

            return new LolitaValuing<TEntity, TProperty> {Inner = self, CurrentField = factory.ParseField(sqlField)};
        }
    }
}