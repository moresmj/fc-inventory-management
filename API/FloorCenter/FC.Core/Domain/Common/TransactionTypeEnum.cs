namespace FC.Core.Domain.Common
{
    /// <summary>
    /// Represents the transaction type
    /// </summary>
    public enum TransactionTypeEnum
    {

        PhysicalCount = 1,

        PO,

        Request,

        Return,

        Sales,

        Transfer,

        AdvanceOrder

    }
}
