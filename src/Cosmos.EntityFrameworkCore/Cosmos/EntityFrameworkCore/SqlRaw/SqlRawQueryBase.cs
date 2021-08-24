using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

// ReSharper disable InconsistentNaming

/*
 * Reference to:
 *     https://github.com/PaulARoy/EntityFrameworkCore.RawSQLExtensions
 * Author:
 *     Paul Roy
 */

namespace Cosmos.EntityFrameworkCore.SqlRaw
{
    public abstract class SqlRawQueryBase<T> : ISqlRawQuery<T>
    {
        protected DatabaseFacade _databaseFacade;
        protected DbParameter[] _sqlParameters;

        public SqlRawQueryBase(DatabaseFacade databaseFacade, params DbParameter[] sqlParameters)
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

        private Task<U> ExecuteAsync<U>(Func<DbDataReader, Task<U>> databaseReaderAction)
        {
            return SqlRawWorker.ExecuteQueryAsync(
                _databaseFacade.GetDbConnection,
                databaseReaderAction,
                InitCommand,
                _sqlParameters);
        }

        private U Execute<U>(Func<DbDataReader, U> databaseReaderAction)
        {
            return SqlRawWorker.ExecuteQuery(
                _databaseFacade.GetDbConnection,
                databaseReaderAction,
                InitCommand,
                _sqlParameters);
        }
    }
}