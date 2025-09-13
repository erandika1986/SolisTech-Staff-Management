using System.ComponentModel;

namespace StaffApp.Domain.Enum
{
    public enum TransportDirection
    {
        [Description("Incoming")]
        Incoming = 1,

        [Description("OutGoing")]
        OutGoing = 2
    }
}
