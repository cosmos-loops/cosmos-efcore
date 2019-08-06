using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Cosmos.Data;
using Cosmos.Data.Transaction;
using Microsoft.EntityFrameworkCore;
using Nito.AsyncEx.Synchronous;

namespace Cosmos.EntityFrameworkCore
{
    /// <summary>
    /// DbContext base
    /// </summary>
    public abstract class DbContextBase : DbContext, IDbContext
    {
        /// <summary>
        /// DbContext base
        /// </summary>
        /// <param name="options"></param>
        /// <param name="transactionCallingWrapper"></param>
        protected DbContextBase(DbContextOptions options, ITransactionCallingWrapper transactionCallingWrapper) : base(options)
        {
            TransactionCallingWrapper = transactionCallingWrapper ?? NullTransactionCallingWrapper.Instance;
        }

        #region Database and connection

        /// <summary>
        /// Transaction calling wrapper
        /// </summary>
        protected ITransactionCallingWrapper TransactionCallingWrapper { get; }

        /// <summary>
        /// Internal connection...
        /// </summary>
        /// <returns></returns>
        protected IDbConnection InternalConnection() => Database.GetDbConnection();

        private bool IsTransCallingWrapperWorking()
        {
            return TransactionCallingWrapper != null && TransactionCallingWrapper.Count > 0;
        }

        #endregion

        #region Before save changes

        /// <summary>
        /// Save change before
        /// </summary>
        // ReSharper disable once VirtualMemberNeverOverridden.Global
        protected virtual void SaveChangesBefore() { }

        #endregion

        #region Save changes

        /// <summary>
        /// Save changes
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
            SaveChangesBefore();
            if (IsTransCallingWrapperWorking())
                return TransactionCommit(TransactionCallingWrapper);
            return base.SaveChanges();
        }

        /// <summary>
        /// Save changes async
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveChangesBefore();
            if (IsTransCallingWrapperWorking())
                return await TransactionCommitAsync(TransactionCallingWrapper, cancellationToken);
            return await base.SaveChangesAsync(cancellationToken);
        }

        #endregion

        #region Commit

        /// <summary>
        /// Commit
        /// </summary>
        public void Commit()
        {
            Commit(null);
        }

        /// <summary>
        /// Commit
        /// </summary>
        /// <param name="callback"></param>
        /// <exception cref="ConcurrencyException"></exception>
        public void Commit(Action callback)
        {
            try
            {
                SaveChanges();
                callback?.Invoke();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new ConcurrencyException(ex);
            }
        }

        /// <summary>
        /// Commit async
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task CommitAsync(CancellationToken cancellationToken = default)
        {
            return Task.Run(() => Commit(null), cancellationToken);
        }

        /// <summary>
        /// Commit async
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ConcurrencyException"></exception>
        public async Task CommitAsync(Action callback, CancellationToken cancellationToken = default)
        {
            try
            {
                await SaveChangesAsync(cancellationToken);
                callback?.Invoke();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new ConcurrencyException(ex);
            }
        }

        private int TransactionCommit(ITransactionCallingWrapper callingWrapper)
        {
            int result;

            using (var connection = (DbConnection) InternalConnection())
            {
                var transactionWrapper = new TransactionWrapper(connection);

                using (transactionWrapper.Begin())
                {
                    try
                    {
                        callingWrapper.CommitAsync(transactionWrapper.CurrentTransaction).WaitAndUnwrapException();
                        Database.UseTransaction((DbTransaction) transactionWrapper.CurrentTransaction);
                        result = base.SaveChanges();
                        transactionWrapper.Commit();
                    }
                    catch
                    {
                        transactionWrapper.Rollback();
                        throw;
                    }
                }
            }

            return result;
        }

        private async Task<int> TransactionCommitAsync(
            ITransactionCallingWrapper callingWrapper,
            CancellationToken cancellationToken = default)
        {
            int result;

            using (var connection = (DbConnection) InternalConnection())
            {
                var transactionWrapper = new TransactionWrapper(connection);

                using (transactionWrapper.Begin())
                {
                    try
                    {
                        await callingWrapper.CommitAsync(transactionWrapper.CurrentTransaction);
                        Database.UseTransaction((DbTransaction) transactionWrapper.CurrentTransaction);
                        result = await base.SaveChangesAsync(cancellationToken);
                        transactionWrapper.Commit();
                    }
                    catch
                    {
                        transactionWrapper.Rollback();
                        throw;
                    }
                }
            }

            return result;
        }

        #endregion

    }
}