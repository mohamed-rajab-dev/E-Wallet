using System;
using System.Collections.Generic;
using System.Text;

namespace E_Wallet.Domain.Enums
{
    public enum OtpStatus
    {
        Used,
        Expired,
        Pending,
        Revoked
    }
}
