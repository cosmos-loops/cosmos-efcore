using System;
using Microsoft.EntityFrameworkCore.Storage;
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
    /// Extensions for Valuing
    /// </summary>
    public static class ValuingExtensions
    {
        /// <summary>
        /// With sql
        /// </summary>
        /// <param name="self"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        // ReSharper disable once InconsistentNaming
        public static LolitaSetting<TEntity> WithSQL<TEntity, TProperty>(this LolitaValuing<TEntity, TProperty> self, Func<string, ISqlGenerationHelper, string> sql,
            params object[] parameters)
        where TEntity : class, new()
        {
            if (sql == null)
            {
                throw new ArgumentNullException(nameof(sql));
            }

            var field = self.CurrentField;
            var sqlGenerationHelper = self.Inner.GetService<ISqlGenerationHelper>();
            var currentSql = sql(field, sqlGenerationHelper);

            var paramCnt = self.Inner.Parameters.Count;
            if (parameters != null && parameters.Length > 0)
            {
                for (var i = 0; i < parameters.Length; i++)
                {
                    currentSql = currentSql.Replace("{" + i + "}", "{" + (i + paramCnt) + "}");
                    self.Inner.Parameters.Add(parameters[i]);
                }
            }

            var fieldInfo = new SetFieldInfo {Field = self.CurrentField, Index = -1, Type = "WithSQL", Value = currentSql};
            var factory = self.GetService<ISetFieldSqlGenerator>();
            self.Inner.Operations.Add(factory.TranslateToSql(fieldInfo));
            return self.Inner;
        }

        /// <summary>
        /// With SQL
        /// </summary>
        /// <param name="self"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        public static LolitaSetting<TEntity> WithSQL<TEntity, TProperty>(this LolitaValuing<TEntity, TProperty> self, Func<string, string> sql, params object[] parameters)
        where TEntity : class, new()
        {
            return self.WithSQL((x, y) => sql(x));
        }

        private static LolitaSetting<TEntity> Valuing<TEntity, TProperty>(this LolitaValuing<TEntity, TProperty> self, string type, object value)
        where TEntity : class, new()
        {
            var factory = self.GetService<ISetFieldSqlGenerator>();
            self.Inner.Parameters.Add(value);
            var sql = factory.TranslateToSql(new SetFieldInfo {Field = self.CurrentField, Index = self.Inner.Parameters.Count - 1, Type = type, Value = value});
            self.Inner.Operations.Add(sql);
            return self.Inner;
        }

        /// <summary>
        /// With value
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> WithValue<TEntity, TProperty>(this LolitaValuing<TEntity, TProperty> self, object value)
        where TEntity : class, new()
            => self.Valuing("WithValue", value);

        /// <summary>
        /// Plus
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> Plus<TEntity>(this LolitaValuing<TEntity, long> self, object value)
        where TEntity : class, new()
            => self.Valuing("Plus", value);

        /// <summary>
        /// Subtract
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> Subtract<TEntity>(this LolitaValuing<TEntity, long> self, object value)
        where TEntity : class, new()
            => self.Valuing("Subtract", value);

        /// <summary>
        /// Multiply
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> Multiply<TEntity>(this LolitaValuing<TEntity, long> self, object value)
        where TEntity : class, new()
            => self.Valuing("Multiply", value);

        /// <summary>
        /// Divide
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> Divide<TEntity>(this LolitaValuing<TEntity, long> self, object value)
        where TEntity : class, new()
            => self.Valuing("Divide", value);

        /// <summary>
        /// Mod
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> Mod<TEntity>(this LolitaValuing<TEntity, long> self, object value)
        where TEntity : class, new()
            => self.Valuing("Mod", value);

        /// <summary>
        /// Plus
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> Plus<TEntity>(this LolitaValuing<TEntity, int> self, object value)
        where TEntity : class, new()
            => self.Valuing("Plus", value);

        /// <summary>
        /// Subtract
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> Subtract<TEntity>(this LolitaValuing<TEntity, int> self, object value)
        where TEntity : class, new()
            => self.Valuing("Subtract", value);

        /// <summary>
        /// Multiply
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> Multiply<TEntity>(this LolitaValuing<TEntity, int> self, object value)
        where TEntity : class, new()
            => self.Valuing("Multiply", value);

        /// <summary>
        /// Divide
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> Divide<TEntity>(this LolitaValuing<TEntity, int> self, object value)
        where TEntity : class, new()
            => self.Valuing("Divide", value);

        /// <summary>
        /// Mod
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> Mod<TEntity>(this LolitaValuing<TEntity, int> self, object value)
        where TEntity : class, new()
            => self.Valuing("Mod", value);

        /// <summary>
        /// Plus
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> Plus<TEntity>(this LolitaValuing<TEntity, short> self, object value)
        where TEntity : class, new()
            => self.Valuing("Plus", value);

        /// <summary>
        /// Subtract
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> Subtract<TEntity>(this LolitaValuing<TEntity, short> self, object value)
        where TEntity : class, new()
            => self.Valuing("Subtract", value);

        /// <summary>
        /// Multiply
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> Multiply<TEntity>(this LolitaValuing<TEntity, short> self, object value)
        where TEntity : class, new()
            => self.Valuing("Multiply", value);

        /// <summary>
        /// Divide
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> Divide<TEntity>(this LolitaValuing<TEntity, short> self, object value)
        where TEntity : class, new()
            => self.Valuing("Divide", value);

        /// <summary>
        /// Mod
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> Mod<TEntity>(this LolitaValuing<TEntity, short> self, object value)
        where TEntity : class, new()
            => self.Valuing("Mod", value);

        /// <summary>
        /// Plus
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> Plus<TEntity>(this LolitaValuing<TEntity, ulong> self, object value)
        where TEntity : class, new()
            => self.Valuing("Plus", value);

        /// <summary>
        /// Subtract
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> Subtract<TEntity>(this LolitaValuing<TEntity, ulong> self, object value)
        where TEntity : class, new()
            => self.Valuing("Subtract", value);

        /// <summary>
        /// Multiply
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> Multiply<TEntity>(this LolitaValuing<TEntity, ulong> self, object value)
        where TEntity : class, new()
            => self.Valuing("Multiply", value);

        /// <summary>
        /// Divide
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> Divide<TEntity>(this LolitaValuing<TEntity, ulong> self, object value)
        where TEntity : class, new()
            => self.Valuing("Divide", value);

        /// <summary>
        /// Mod
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> Mod<TEntity>(this LolitaValuing<TEntity, ulong> self, object value)
        where TEntity : class, new()
            => self.Valuing("Mod", value);

        /// <summary>
        /// Plus
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> Plus<TEntity>(this LolitaValuing<TEntity, uint> self, object value)
        where TEntity : class, new()
            => self.Valuing("Plus", value);

        /// <summary>
        /// Subtract
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> Subtract<TEntity>(this LolitaValuing<TEntity, uint> self, object value)
        where TEntity : class, new()
            => self.Valuing("Subtract", value);

        /// <summary>
        /// Multiply
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> Multiply<TEntity>(this LolitaValuing<TEntity, uint> self, object value)
        where TEntity : class, new()
            => self.Valuing("Multiply", value);

        /// <summary>
        /// Divide
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> Divide<TEntity>(this LolitaValuing<TEntity, uint> self, object value)
        where TEntity : class, new()
            => self.Valuing("Divide", value);

        /// <summary>
        /// Mod
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> Mod<TEntity>(this LolitaValuing<TEntity, uint> self, object value)
        where TEntity : class, new()
            => self.Valuing("Mod", value);

        /// <summary>
        /// Plus
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> Plus<TEntity>(this LolitaValuing<TEntity, ushort> self, object value)
        where TEntity : class, new()
            => self.Valuing("Plus", value);

        /// <summary>
        /// Subtract
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> Subtract<TEntity>(this LolitaValuing<TEntity, ushort> self, object value)
        where TEntity : class, new()
            => self.Valuing("Subtract", value);

        /// <summary>
        /// Multiply
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> Multiply<TEntity>(this LolitaValuing<TEntity, ushort> self, object value)
        where TEntity : class, new()
            => self.Valuing("Multiply", value);

        /// <summary>
        /// Divide
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> Divide<TEntity>(this LolitaValuing<TEntity, ushort> self, object value)
        where TEntity : class, new()
            => self.Valuing("Divide", value);

        /// <summary>
        /// Mod
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> Mod<TEntity>(this LolitaValuing<TEntity, ushort> self, object value)
        where TEntity : class, new()
            => self.Valuing("Mod", value);

        /// <summary>
        /// Plus
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> Plus<TEntity>(this LolitaValuing<TEntity, double> self, object value)
        where TEntity : class, new()
            => self.Valuing("Plus", value);

        /// <summary>
        /// Subtract
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> Subtract<TEntity>(this LolitaValuing<TEntity, double> self, object value)
        where TEntity : class, new()
            => self.Valuing("Subtract", value);

        /// <summary>
        /// Multiply
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> Multiply<TEntity>(this LolitaValuing<TEntity, double> self, object value)
        where TEntity : class, new()
            => self.Valuing("Multiply", value);

        /// <summary>
        /// Divide
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> Divide<TEntity>(this LolitaValuing<TEntity, double> self, object value)
        where TEntity : class, new()
            => self.Valuing("Divide", value);

        /// <summary>
        /// Mod
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> Mod<TEntity>(this LolitaValuing<TEntity, double> self, object value)
        where TEntity : class, new()
            => self.Valuing("Mod", value);

        /// <summary>
        /// Plus
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> Plus<TEntity>(this LolitaValuing<TEntity, float> self, object value)
        where TEntity : class, new()
            => self.Valuing("Plus", value);

        /// <summary>
        /// Subtract
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> Subtract<TEntity>(this LolitaValuing<TEntity, float> self, object value)
        where TEntity : class, new()
            => self.Valuing("Subtract", value);

        /// <summary>
        /// Multiply
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> Multiply<TEntity>(this LolitaValuing<TEntity, float> self, object value)
        where TEntity : class, new()
            => self.Valuing("Multiply", value);

        /// <summary>
        /// Divid
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> Divide<TEntity>(this LolitaValuing<TEntity, float> self, object value)
        where TEntity : class, new()
            => self.Valuing("Divide", value);

        /// <summary>
        /// Mod
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> Mod<TEntity>(this LolitaValuing<TEntity, float> self, object value)
        where TEntity : class, new()
            => self.Valuing("Mod", value);

        /// <summary>
        /// Append
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> Append<TEntity>(this LolitaValuing<TEntity, string> self, string value)
        where TEntity : class, new()
            => self.Valuing("Append", value);

        /// <summary>
        /// Prepend
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> Prepend<TEntity>(this LolitaValuing<TEntity, string> self, string value)
        where TEntity : class, new()
            => self.Valuing("Prepend", value);

        /// <summary>
        /// Add milliseconds
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> AddMilliseconds<TEntity>(this LolitaValuing<TEntity, DateTime> self, int value)
        where TEntity : class, new()
            => self.Valuing("AddMilliseconds", value);

        /// <summary>
        /// Add seconds
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> AddSeconds<TEntity>(this LolitaValuing<TEntity, DateTime> self, int value)
        where TEntity : class, new()
            => self.Valuing("AddSeconds", value);

        /// <summary>
        /// Add minutes
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> AddMinutes<TEntity>(this LolitaValuing<TEntity, DateTime> self, int value)
        where TEntity : class, new()
            => self.Valuing("AddMinutes", value);

        /// <summary>
        /// Add hours
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> AddHours<TEntity>(this LolitaValuing<TEntity, DateTime> self, int value)
        where TEntity : class, new()
            => self.Valuing("AddHours", value);

        /// <summary>
        /// Add days
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> AddDays<TEntity>(this LolitaValuing<TEntity, DateTime> self, int value)
        where TEntity : class, new()
            => self.Valuing("AddDays", value);

        /// <summary>
        /// Add months
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> AddMonths<TEntity>(this LolitaValuing<TEntity, DateTime> self, int value)
        where TEntity : class, new()
            => self.Valuing("AddMonths", value);

        /// <summary>
        /// Add years
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static LolitaSetting<TEntity> AddYears<TEntity>(this LolitaValuing<TEntity, DateTime> self, int value)
        where TEntity : class, new()
            => self.Valuing("AddYears", value);
    }
}