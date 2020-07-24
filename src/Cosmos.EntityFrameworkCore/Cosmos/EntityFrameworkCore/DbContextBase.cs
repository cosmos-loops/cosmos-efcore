using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Cosmos.Data;
using Cosmos.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Cosmos.EntityFrameworkCore
{
    /// <summary>
    /// DbContext base
    /// </summary>
    public abstract class DbContextBase : DbContext, IEfContext
    {
        /// <summary>
        /// DbContext base
        /// </summary>
        /// <param name="options"></param>
        protected DbContextBase(DbContextOptions options) : base(options) { }

        #region Database and connection

        private ITransactionWrapper _transactionWrapper;

        internal ITransactionWrapper Transaction
        {
            get
            {
                if (_transactionWrapper is null)
                    _transactionWrapper = new TransactionWrapper(CurrentConnection);
                return _transactionWrapper;
            }
        }

        /// <summary>
        /// Internal connection...
        /// </summary>
        /// <returns></returns>
        protected DbConnection CurrentConnection => Database.GetDbConnection();

        #endregion

        #region SaveChanges

        /// <summary>
        /// On saving changes
        /// </summary>
        protected virtual void OnSavingChanges() { }

        /// <summary>
        /// On saved changes
        /// </summary>
        protected virtual void OnSavedChanges() { }

        /// <summary>
        /// Save changes
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
            OnSavingChanges();
            var ret = base.SaveChanges();
            OnSavedChanges();
            return ret;
        }

        /// <summary>
        /// Save changes async
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            OnSavingChanges();
            var ret = await base.SaveChangesAsync(cancellationToken);
            OnSavedChanges();
            return ret;
        }

        #endregion

        #region Commit

        /// <summary>
        /// Commit
        /// </summary>
        public void Commit() => Commit(null);

        /// <summary>
        /// Commit async
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task CommitAsync(CancellationToken cancellationToken = default) => CommitAsync(null, cancellationToken);

        /// <inheritdoc />
        public void Rollback() => _transactionWrapper?.Rollback();

        /// <summary>
        /// Commit
        /// </summary>
        /// <param name="callback"></param>
        /// <exception cref="ConcurrencyException"></exception>
        public void Commit(Action callback)
        {
            try
            {
                Database.UseTransaction(Transaction.GetOrBegin());
                SaveChanges(true);
                callback?.Invoke();
                Transaction.Commit();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Rollback();
                throw new ConcurrencyException(ex);
            }
            catch
            {
                Rollback();
                throw;
            }
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
                Database.UseTransaction(Transaction.GetOrBegin());
                await SaveChangesAsync(true, cancellationToken);
                callback?.Invoke();
                Transaction.Commit();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Rollback();
                throw new ConcurrencyException(ex);
            }
            catch
            {
                Rollback();
                throw;
            }
        }

        #endregion
    }
}