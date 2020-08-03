using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

// ReSharper disable InconsistentNaming

/*
 * Reference to:
 *     https://github.com/PaulARoy/EntityFrameworkCore.RawSQLExtensions
 * Author:
 *     Paul Roy
 */

namespace Cosmos.EntityFrameworkCore.Core.RawQuery
{
    public abstract class SqlQueryBase<T> : ISqlQuery<T>
    {
        protected DatabaseFacade _databaseFacade;
        protected DbParameter[] _sqlParameters;

        public SqlQueryBase(DatabaseFacade databaseFacade, params DbParameter[] sqlParameters)
        {
            _databaseFacade = databaseFacade;
            _sqlParameters = sqlParameters;
        }

        public async Task<IList<T>> ToListAsync()
        {
            return await ExecuteAsync(dbReader => dbReader.ToListAsync<T>());
        }

        public async Task<T> FirstOrDefaultAsync()
        {
            return await ExecuteAsync(dbReader => dbReader.FirstOrDefaultAsync<T>());
        }

        public async Task<T> SingleOrDefaultAsync()
        {
            return await ExecuteAsync(dbReader => dbReader.SingleOrDefaultAsync<T>());
        }

        public async Task<T> FirstAsync()
        {
            var result = await FirstOrDefaultAsync();
            if (result == null)
                throw new InvalidOperationException("Sequence contains no elements");

            return result;
        }

        public async Task<T> SingleAsync()
        {
            var result = await SingleOrDefaultAsync();
            if (result == null)
                throw new InvalidOperationException("Sequence contains no elements");

            return result;
        }

        public IList<T> ToList()
        {
            return Execute(dbReader => dbReader.ToList<T>());
        }

        public DataTable ToDataTable()
        {
            return Execute(dbReader => dbReader.ToDataTable());
        }

        public async Task<DataTable> ToDataTableAsync()
        {
            return await ExecuteAsync(dbReader => dbReader.ToDataTableAsync());
        }

        public T FirstOrDefault()
        {
            return Execute(dbReader => dbReader.FirstOrDefault<T>());
        }

        public T SingleOrDefault()
        {
            return Execute(dbReader => dbReader.SingleOrDefault<T>());
        }

        public T First()
        {
            var result = FirstOrDefault();
            if (result == null)
                throw new InvalidOperationException("Sequence contains no elements");

            return result;
        }

        public T Single()
        {
            var result = SingleOrDefault();
            if (result == null)
                throw new InvalidOperationException("Sequence contains no elements");

            return result;
        }

        // customization of command (sql / stored procedure)
        protected abstract void InitCommand(DbCommand command);

        private async Task<U> ExecuteAsync<U>(Func<DbDataReader, Task<U>> databaseReaderAction)
        {
            U result = default;

            var conn = _databaseFacade.GetDbConnection();
            try
            {
                await conn.OpenAsync();
                await using var command = conn.CreateCommand();
                InitCommand(command);

                foreach (var param in _sqlParameters)
                {
                    var p = command.CreateParameter();
                    p.ParameterName = param.ParameterName;
                    p.Value = param.Value;
                    command.Parameters.Add(p);
                }

                await using var reader = await command.ExecuteReaderAsync();
                result = await databaseReaderAction.Invoke(reader);
            }
            finally
            {
                await conn.CloseAsync();
            }

            return result;
        }

        private U Execute<U>(Func<DbDataReader, U> databaseReaderAction)
        {
            U result = default;

            var conn = _databaseFacade.GetDbConnection();
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                using var command = conn.CreateCommand();
                InitCommand(command);

                foreach (var param in _sqlParameters)
                {
                    var p = command.CreateParameter();
                    p.ParameterName = param.ParameterName;
                    p.Value = param.Value;
                    command.Parameters.Add(p);
                }

                using var reader = command.ExecuteReader();
                result = databaseReaderAction.Invoke(reader);
            }
            finally
            {
                conn.Close();
            }

            return result;
        }
    }
}