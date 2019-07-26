using Cosmos.Data.Transaction;
using Microsoft.EntityFrameworkCore.Storage;

namespace Cosmos.EntityFrameworkCore
{
    public static class DbContextTransactionExtensions
    {
        public static ITransactionWrapper ToTransactionWrapper(this IDbContextTransaction transaction)
        {
            transaction.CheckNull(nameof(transaction));
            return TransactionWrapper.CreateFromTransaction(transaction.GetDbTransaction());
        }

        public static ITransactionWrapper ToTransactionWrapper(this IDbContextTransaction transaction, TransactionAppendOperator @operator)
        {
            transaction.CheckNull(nameof(transaction));
            return TransactionWrapper.CreateFromTransaction(transaction.GetDbTransaction(), @operator);
        }
    }
}