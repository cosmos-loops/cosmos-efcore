using Cosmos.Data.Common;
using Microsoft.EntityFrameworkCore.Storage;

namespace Cosmos.EntityFrameworkCore
{
    /// <summary>
    /// Extensions fot DbContext transaction
    /// </summary>
    public static class CosmosDbContextTransactionExtensions
    {
        /// <summary>
        /// To traction wrapper
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public static ITransactionWrapper ToTransactionWrapper(this IDbContextTransaction transaction)
        {
            transaction.CheckNull(nameof(transaction));
            return TransactionWrapper.CreateFromTransaction(transaction.GetDbTransaction());
        }

        /// <summary>
        /// To traction wrapper
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="operator"></param>
        /// <returns></returns>
        public static ITransactionWrapper ToTransactionWrapper(this IDbContextTransaction transaction, TransactionAppendOperator @operator)
        {
            transaction.CheckNull(nameof(transaction));
            return TransactionWrapper.CreateFromTransaction(transaction.GetDbTransaction(), @operator);
        }
    }
}