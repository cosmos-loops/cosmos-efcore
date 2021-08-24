using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace Cosmos.EntityFrameworkCore.SqlRaw
{
    internal static partial class SqlRawWorker
    {
        public static U ExecuteQuery<U>(
            Func<DbConnection> connectionFactory,
            Func<DbDataReader, U> readerAction,
            Action<DbCommand> initCommandAction,
            DbParameter[] sqlParameters)
        {
            U result;

            var conn = connectionFactory();
            var needAutoCloseConn = false;

            try
            {
                needAutoCloseConn = conn.OpenIfNeeded();

                using (var command = conn.CreateCommand())
                {
                    initCommandAction(command);

                    foreach (var param in sqlParameters)
                    {
                        command.AppendParameter(param);
                    }

                    using (var reader = command.ExecuteReader())
                    {
                        result = readerAction.Invoke(reader);
                    }
                }
            }
            finally
            {
                if (needAutoCloseConn)
                {
                    conn.Close();
                }
            }

            return result;
        }

        public static async Task<U> ExecuteQueryAsync<U>(
            Func<DbConnection> connectionFactory,
            Func<DbDataReader, Task<U>> databaseReaderAction,
            Action<DbCommand> initCommandAction,
            DbParameter[] sqlParameters,
            CancellationToken cancellationToken = default)
        {
            U result;

            var conn = connectionFactory();
            var needAutoCloseConn = false;

            try
            {
                needAutoCloseConn = await conn.OpenIfNeededAsync(cancellationToken);

                await using (var command = conn.CreateCommand())
                {
                    initCommandAction(command);

                    foreach (var param in sqlParameters)
                    {
                        command.AppendParameter(param);
                    }

                    await using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                    {
                        result = await databaseReaderAction.Invoke(reader);
                    }
                }
            }
            finally
            {
                if (needAutoCloseConn)
                {
                    await conn.CloseAsync();
                }
            }

            return result;
        }

        public static int Execute(
            string sqlText,
            Func<DbConnection> connectionFactory,
            Func<IRelationalTypeMappingSource> typeMappingSourceFactory,
            Func<IDbContextTransaction> transactionFactory,
            IReadOnlyDictionary<string, object> parameters)
        {
            var conn = connectionFactory();
            var needAutoCloseConn = false;

            try
            {
                needAutoCloseConn = conn.OpenIfNeeded();

                using (var command = conn.CreateCommand())
                {
                    command.ApplyCurrentTransaction(transactionFactory());
                    command.CommandText = sqlText;
                    command.AppendParameters(typeMappingSourceFactory, parameters);
                    return command.ExecuteNonQuery();
                }
            }
            finally
            {
                if (needAutoCloseConn)
                {
                    conn.Close();
                }
            }
        }

        public static async Task<int> ExecuteAsync(
            string sqlText,
            Func<DbConnection> connectionFactory,
            Func<IRelationalTypeMappingSource> typeMappingSourceFactory,
            Func<IDbContextTransaction> transactionFactory,
            IReadOnlyDictionary<string, object> parameters,
            CancellationToken cancellationToken = default)
        {
            var conn = connectionFactory();
            var needAutoCloseConn = false;

            try
            {
                needAutoCloseConn = await conn.OpenIfNeededAsync(cancellationToken);

                await using (var command = conn.CreateCommand())
                {
                    command.ApplyCurrentTransaction(transactionFactory());
                    command.CommandText = sqlText;
                    command.AppendParameters(typeMappingSourceFactory, parameters);
                    return await command.ExecuteNonQueryAsync(cancellationToken);
                }
            }
            finally
            {
                if (needAutoCloseConn)
                {
                    await conn.CloseAsync();
                }
            }
        }

        private static void AppendParameter(this DbCommand command, DbParameter parameter)
        {
            var p = command.CreateParameter();

            p.ParameterName = parameter.ParameterName;
            p.Value = parameter.Value;

            command.Parameters.Add(p);
        }

        private static void AppendParameter(this DbCommand command, KeyValuePair<string, object> parameter)
        {
            var p = command.CreateParameter();

            p.ParameterName = parameter.Key;
            p.Value = parameter.Value;

            command.Parameters.Add(p);
        }

        private static void AppendParameters(this DbCommand command, Func<IRelationalTypeMappingSource> typeMappingSourceFactory,
            IReadOnlyDictionary<string, object> parameters)
        {
            var typeMapping = typeMappingSourceFactory();

            foreach (var p in parameters)
            {
                if (p.Value is not null && typeMapping.FindMapping(p.Value.GetType()) is null)
                    continue;

                command.AppendParameter(p);
            }
        }

        private static void ApplyCurrentTransaction(this IDbCommand cmd, IDbContextTransaction transaction)
        {
            if (transaction is not null)
            {
                cmd.Transaction = transaction.GetDbTransaction();
            }
        }

        internal static bool OpenIfNeeded(this DbConnection conn)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
                return true;
            }

            return false;
        }

        internal static async Task<bool> OpenIfNeededAsync(this DbConnection conn, CancellationToken cancellationToken = default)
        {
            if (conn.State != ConnectionState.Open)
            {
                await conn.OpenAsync(cancellationToken);
                return true;
            }

            return false;
        }
    }
}