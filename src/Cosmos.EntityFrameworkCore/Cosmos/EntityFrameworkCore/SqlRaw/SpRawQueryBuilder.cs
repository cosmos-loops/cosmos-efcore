using System;
using System.Data.Common;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Cosmos.EntityFrameworkCore.SqlRaw
{
    public interface ISpRawQueryBuilder<T>
    {
        ISpRawQueryLevel2Builder<T> With(string storedProcedureName);
    }

    public interface ISpRawQueryLevel2Builder<T>
    {
        ISpRawQueryLevel3Builder<T> AndWith(DbParameter[] sqlParameters);

        SpRawQuery<T> Build();
    }

    public interface ISpRawQueryLevel3Builder<T>
    {
        SpRawQuery<T> Build();
    }

    public class SpRawQueryBuilder<T> : ISpRawQueryBuilder<T>, ISpRawQueryLevel2Builder<T>, ISpRawQueryLevel3Builder<T>
    {
        private readonly DatabaseFacade _databaseFacade;
        private string _storedProcedureName;
        private DbParameter[] _sqlParameters;

        public SpRawQueryBuilder(DatabaseFacade databaseFacade)
        {
            _databaseFacade = databaseFacade ?? throw new ArgumentNullException(nameof(databaseFacade));
        }

        public ISpRawQueryLevel2Builder<T> With(string storedProcedureName)
        {
            _storedProcedureName = storedProcedureName;
            return this;
        }

        public ISpRawQueryLevel3Builder<T> AndWith(DbParameter[] sqlParameters)
        {
            _sqlParameters = sqlParameters;
            return this;
        }

        public SpRawQuery<T> Build()
        {
            return new SpRawQuery<T>(_databaseFacade, _storedProcedureName, _sqlParameters);
        }
    }
}