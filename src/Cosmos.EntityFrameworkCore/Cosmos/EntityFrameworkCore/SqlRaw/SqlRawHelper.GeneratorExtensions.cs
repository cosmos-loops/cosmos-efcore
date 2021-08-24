using System.Text;

// ReSharper disable InconsistentNaming

namespace Cosmos.EntityFrameworkCore.SqlRaw
{
    internal static partial class SqlGeneratorExtensions
    {
        public static StringBuilder SELECT(this StringBuilder sqlBuilder)
        {
            sqlBuilder.Append("SELECT ");
            return sqlBuilder;
        }

        public static StringBuilder DISTINCT(this StringBuilder sqlBuilder)
        {
            sqlBuilder.Append("DISTINCT ");
            return sqlBuilder;
        }

        public static StringBuilder UPDATE(this StringBuilder sqlBuilder, string tableName)
        {
            sqlBuilder.Append($"UPDATE {tableName} SET ");
            return sqlBuilder;
        }

        public static StringBuilder DELETE(this StringBuilder sqlBuilder)
        {
            sqlBuilder.Append("DELETE ");
            return sqlBuilder;
        }

        public static StringBuilder FROM(this StringBuilder sqlBuilder, string tableName)
        {
            sqlBuilder.Append($"FROM {tableName} ");
            return sqlBuilder;
        }
        
        public static StringBuilder WHERE(this StringBuilder sqlBuilder, string whereString)
        {
            sqlBuilder.Append($"WHERE {whereString} ");
            return sqlBuilder;
        }

        public static StringBuilder NEWLINE(this StringBuilder sqlBuilder)
        {
            sqlBuilder.AppendLine();
            return sqlBuilder;
        }
    }
}