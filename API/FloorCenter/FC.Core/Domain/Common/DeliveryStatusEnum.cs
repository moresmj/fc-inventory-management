namespace FC.Core.Domain.Common
{
    /// <summary>
    /// Represents the delivery status
    /// </summary>
    public enum DeliveryStatusEnum
    {

        Delivered = 1,

        Pending,

        Waiting,

        NotDelivered,

        WaitingForConfirmation

    }
}
