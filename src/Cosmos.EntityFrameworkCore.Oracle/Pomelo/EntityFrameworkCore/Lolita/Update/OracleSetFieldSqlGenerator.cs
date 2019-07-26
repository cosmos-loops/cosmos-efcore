using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Storage;

namespace Pomelo.EntityFrameworkCore.Lolita.Update
{
    public class OracleSetFieldSqlGenerator : DefaultSetFieldSqlGenerator
    {
        public OracleSetFieldSqlGenerator(ISqlGenerationHelper x) : base(x) { }

        public override string TranslateToSql(SetFieldInfo operation)
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
                    return $"{operation.Field} = MOD({operation.Field}, {{{operation.Index}}})";
                case "Append":
                    return $"{operation.Field} = {operation.Field}||{{{operation.Index}}}";
                case "Prepend":
                    return $"{operation.Field} = {{{operation.Index}}}||{operation.Field}";
                case "AddMilliseconds":
                    throw new NotSupportedException("Oracle does not support million seconds operation of a datetime type.");
                case "AddSeconds":
                    return $"{operation.Field} = {operation.Field} + {{{operation.Index}}} / (24 * 60 * 60)";
                case "AddMinutes":
                    return $"{operation.Field} = {operation.Field} + {{{operation.Index}}} / (24 * 60)";
                case "AddHours":
                    return $"{operation.Field} = {operation.Field} + {{{operation.Index}}} / 24";
                case "AddDays":
                    return $"{operation.Field} = {operation.Field} + {{{operation.Index}}}";
                case "AddMonths":
                    return $"{operation.Field} = ADD_MONTHS({operation.Field}, {{{operation.Index}}})";
                case "AddYears":
                    return $"{operation.Field} = ADD_MONTHS({operation.Field}, {{{operation.Index}}} * 12)";
            }

            return string.Empty;
        }
    }
}