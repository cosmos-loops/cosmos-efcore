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
    public abstract class DbContextBase : DbContext, IDbContext
    {
        protected DbContextBase(DbContextOptions options, ITransactionCallingWrapper transactionCallingWrapper) : base(options)
        {
            TransactionCallingWrapper = transactionCallingWrapper ?? NullTransactionCallingWrapper.Instance;
        }

        #region Database and connection

        protected ITransactionCallingWrapper TransactionCallingWrapper { get; }

        protected IDbConnection InternalConnection() => Database.GetDbConnection();

        private bool IsTransCallingWrapperWorking()
        {
            return TransactionCallingWrapper != null && TransactionCallingWrapper.Count > 0;
        }

        #endregion

        #region Before save changes

        // ReSharper disable once VirtualMemberNeverOverridden.Global
        protected virtual void SaveChangesBefore() { }

        #endregion

        #region Save changes

        public override int SaveChanges()
        {
            SaveChangesBefore();
            if (IsTransCallingWrapperWorking())
                return TransactionCommit(TransactionCallingWrapper);
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveChangesBefore();
            if (IsTransCallingWrapperWorking())
                return await TransactionCommitAsync(TransactionCallingWrapper, cancellationToken);
            return await base.SaveChangesAsync(cancellationToken);
        }

        #endregion

        #region Commit

        public void Commit()
        {
            Commit(null);
        }

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

        public Task CommitAsync(CancellationToken cancellationToken = default)
        {
            return Task.Run(() => Commit(null), cancellationToken);
        }

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
            var result = 0;

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
            var result = 0;

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