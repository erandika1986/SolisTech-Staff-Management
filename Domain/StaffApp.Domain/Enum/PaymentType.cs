using System.ComponentModel;

namespace StaffApp.Domain.Enum
{
    public enum PaymentType
    {
        [Description("Bank Transfer")]
        BankTransfer = 1,

        [Description("Cash")]
        Cash = 2,

        [Description("Cheque")]
        Cheque = 3,
        [Description("Credit Card")]
        CreditCard = 4,

        [Description("Debit Card")]
        DebitCard = 5,

        [Description("Mobile Payment")]
        MobilePayment = 6,

        [Description("Online Payment")]
        OnlinePayment = 7,

        [Description("Other")]
        Other = 8
    }
}
