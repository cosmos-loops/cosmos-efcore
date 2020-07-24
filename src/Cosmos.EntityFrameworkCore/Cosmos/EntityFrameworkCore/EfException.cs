using System;
using System.Data;

// ReSharper disable InconsistentNaming

namespace Cosmos.EntityFrameworkCore
{
    /// <summary>
    /// EfCore exception
    /// </summary>
    public class EfException : CosmosException
    {
        /// <summary>
        /// Default db ctx flag
        /// </summary>
        protected const string DEFAULT_DBCTX_FLAG = "__EF_DBCTX_FLG";

        /// <summary>
        /// Default db ctx error message
        /// </summary>
        protected const string DEFAULT_DBCTX_ERROR_MESSAGE = "_DEFAULT_EF_DBCONTEXT_ERROR";

        /// <summary>
        /// Default db ctx error code
        /// </summary>
        protected const long DEFAULT_DBCTX_ERROR_CODE = 200101;

        /// <summary>
        /// Create a new instance of <see cref="EfException"/>
        /// </summary>
        public EfException()
            : this(null, DEFAULT_DBCTX_ERROR_CODE, DEFAULT_DBCTX_ERROR_MESSAGE, DEFAULT_DBCTX_FLAG) { }

        /// <summary>
        /// Create a new instance of <see cref="EfException"/>
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="innerException"></param>
        public EfException(string errorMessage, Exception innerException = null)
            : this(null, DEFAULT_DBCTX_ERROR_CODE, errorMessage, DEFAULT_DBCTX_FLAG, innerException) { }

        /// <summary>
        /// Create a new instance of <see cref="EfException"/>
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="flag"></param>
        /// <param name="innerException"></param>
        public EfException(string errorMessage, string flag, Exception innerException = null)
            : this(null, DEFAULT_DBCTX_ERROR_CODE, errorMessage, flag, innerException) { }

        /// <summary>
        /// Create a new instance of <see cref="EfException"/>
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="errorMessage"></param>
        /// <param name="innerException"></param>
        public EfException(long errorCode, string errorMessage, Exception innerException = null)
            : this(null, errorCode, errorMessage, DEFAULT_DBCTX_FLAG, innerException) { }

        /// <summary>
        /// Create a new instance of <see cref="EfException"/>
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="errorMessage"></param>
        /// <param name="flag"></param>
        /// <param name="innerException"></param>
        public EfException(long errorCode, string errorMessage, string flag, Exception innerException)
            : this(null, errorCode, errorMessage, flag, innerException) { }

        /// <summary>
        /// Create a new instance of <see cref="EfException"/>
        /// </summary>
        /// <param name="connection"></param>
        public EfException(IDbConnection connection)
            : this(connection, DEFAULT_DBCTX_ERROR_CODE, DEFAULT_DBCTX_ERROR_MESSAGE, DEFAULT_DBCTX_FLAG) { }

        /// <summary>
        /// Create a new instance of <see cref="EfException"/>
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="errorMessage"></param>
        /// <param name="innerException"></param>
        public EfException(IDbConnection connection, string errorMessage, Exception innerException = null)
            : this(connection, DEFAULT_DBCTX_ERROR_CODE, errorMessage, DEFAULT_DBCTX_FLAG, innerException) { }

        /// <summary>
        /// Create a new instance of <see cref="EfException"/>
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="errorMessage"></param>
        /// <param name="flag"></param>
        /// <param name="innerException"></param>
        public EfException(IDbConnection connection, string errorMessage, string flag,
            Exception innerException = null)
            : this(connection, DEFAULT_DBCTX_ERROR_CODE, errorMessage, flag, innerException) { }

        /// <summary>
        /// Create a new instance of <see cref="EfException"/>
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="errorCode"></param>
        /// <param name="errorMessage"></param>
        /// <param name="innerException"></param>
        public EfException(IDbConnection connection, long errorCode, string errorMessage,
            Exception innerException = null)
            : this(connection, errorCode, errorMessage, DEFAULT_DBCTX_FLAG, innerException) { }

        /// <summary>
        /// Create a new instance of <see cref="EfException"/>
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="errorCode"></param>
        /// <param name="errorMessage"></param>
        /// <param name="flag"></param>
        /// <param name="innerException"></param>
        public EfException(IDbConnection connection, long errorCode, string errorMessage, string flag,
            Exception innerException = null)
            : base(errorCode, errorMessage, flag, innerException)
        {
            if (connection != null)
            {
                Database = connection.Database;
                ConnectionString = connection.ConnectionString;
                ConnectionState = connection.State;
            }
        }

        /// <summary>
        /// Gets database
        /// </summary>
        public string Database { get; }

        /// <summary>
        /// Gets connection string
        /// </summary>
        public string ConnectionString { get; }

        /// <summary>
        /// Gets connection state
        /// </summary>
        public ConnectionState ConnectionState { get; }
    }
}