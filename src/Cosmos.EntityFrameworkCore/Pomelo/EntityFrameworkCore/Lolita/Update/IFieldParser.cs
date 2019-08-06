using System;
using System.Linq.Expressions;

namespace Pomelo.EntityFrameworkCore.Lolita.Update
{
    /// <summary>
    /// Interface for field paeser
    /// </summary>
    public interface IFieldParser
    {
        /// <summary>
        /// Visit field
        /// </summary>
        /// <param name="exp"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <returns></returns>
        SqlFieldInfo VisitField<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> exp) where TEntity : class, new();

        /// <summary>
        /// Parser field
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        string ParseField(SqlFieldInfo field);

        /// <summary>
        /// Parser full table
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        string ParseFullTable(SqlFieldInfo field);

        /// <summary>
        /// Parser short table
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        string ParseShortTable(SqlFieldInfo field);
    }
}