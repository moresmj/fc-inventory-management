namespace FC.Core.Domain.Common
{
    public enum PaymentModeEnum
    {
        Cash = 1,

        CreditCard,

        Cheque,

        CashOnDelivery,

        ChequeOnDelivery,

        BankTransfer,

        VoucherCoupon,

        PartialPayment,

        Reservation,

        AccountsReceivable,

        PaymentUponPickup
    }
}
