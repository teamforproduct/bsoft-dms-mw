using System.Transactions;

namespace BL.CrossCutting.Helpers
{
    public class Transactions
    {
        public static TransactionScope GetTransaction() => new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted });

    }
}
