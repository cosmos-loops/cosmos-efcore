using System;
using System.Data.Common;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Cosmos.EntityFrameworkCore.SqlRaw
{
    public interface ISqlRawQueryBuilder<T>
    {
        ISqlRawQueryLevel2Builder<T> With(string sqlQuery);
    }

    public interface ISqlRawQueryLevel2Builder<T>
    {
        ISqlRawQueryLevel3Builder<T> AndWith(DbParameter[] sqlParameters);

        SqlRawQuery<T> Build();
    }

    public interface ISqlRawQueryLevel3Builder<T>
    {
        SqlRawQuery<T> Build();
    }

    public class SqlRawQueryBuilder<T> : ISqlRawQueryBuilder<T>, ISqlRawQueryLevel2Builder<T>, ISqlRawQueryLevel3Builder<T>
    {
        private readonly DatabaseFacade _databaseFacade;
        private string _sqlQuery;
        private DbParameter[] _sqlParameters;

        public SqlRawQueryBuilder(DatabaseFacade databaseFacade)
        {
            _databaseFacade = databaseFacade ?? throw new ArgumentNullException(nameof(databaseFacade));
        }

        public ISqlRawQueryLevel2Builder<T> With(string sqlQuery)
        {
            _sqlQuery = sqlQuery;
            return this;
        }

        public ISqlRawQueryLevel3Builder<T> AndWith(DbParameter[] sqlParameters)
        {
            _sqlParameters = sqlParameters;
            return this;
        }

        public SqlRawQuery<T> Build()
        {
            return new SqlRawQuery<T>(_databaseFacade, _sqlQuery, _sqlParameters);
        }
    }
}