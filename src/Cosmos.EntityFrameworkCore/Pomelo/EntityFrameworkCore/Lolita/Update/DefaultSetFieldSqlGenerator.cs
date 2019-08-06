using System;
using Microsoft.EntityFrameworkCore.Storage;

namespace Pomelo.EntityFrameworkCore.Lolita.Update
{
    /// <summary>
    /// Default set field sql generator
    /// </summary>
    public class DefaultSetFieldSqlGenerator : ISetFieldSqlGenerator
    {
        /// <summary>
        /// Create a new instance of <see cref="DefaultSetFieldSqlGenerator"/>
        /// </summary>
        /// <param name="sqlGenerationHelper"></param>
        public DefaultSetFieldSqlGenerator(ISqlGenerationHelper sqlGenerationHelper)
        {
            SqlGenerationHelper = sqlGenerationHelper;
        }

        /// <summary>
        /// Sql generation helper
        /// </summary>
        protected ISqlGenerationHelper SqlGenerationHelper;

        /// <summary>
        /// Translate to sql
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public virtual string TranslateToSql(SetFieldInfo operation)
        {
            switch (operation.Type)
            {
                case "WithSQL":
                    return $"{operation.Field} = {operation.Value}";
                case "WithValue":
                    return $"{operation.Field} = {{{operation.Index}}}";
                case "Plus":
                    return $"{operation.Field} = {operation.Field} + {{{operation.Index}}}";
                case "Subtract":
                    return $"{operation.Field} = {operation.Field} - {{{operation.Index}}}";
                case "Multiply":
                    return $"{operation.Field} = {operation.Field} * {{{operation.Index}}}";
                case "Divide":
                    return $"{operation.Field} = {operation.Field} / {{{operation.Index}}}";
                case "Mod":
                    return $"{operation.Field} = {operation.Field} % {{{operation.Index}}}";
                case "Append":
                    throw new NotSupportedException("Relational field setter does not support this operation.");
                case "Prepend":
                    throw new NotSupportedException("Relational field setter does not support this operation.");
                case "AddMilliseconds":
                    throw new NotSupportedException("Relational field setter does not support this operation.");
                case "AddSeconds":
                    throw new NotSupportedException("Relational field setter does not support this operation.");
                case "AddMinutes":
                    throw new NotSupportedException("Relational field setter does not support this operation.");
                case "AddHours":
                    throw new NotSupportedException("Relational field setter does not support this operation.");
                case "AddDays":
                    throw new NotSupportedException("Relational field setter does not support this operation.");
                case "AddMonths":
                    throw new NotSupportedException("Relational field setter does not support this operation.");
                case "AddYears":
                    throw new NotSupportedException("Relational field setter does not support this operation.");
            }

            return string.Empty;
        }
    }
}