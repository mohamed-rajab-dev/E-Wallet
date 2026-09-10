using System;
using System.Collections.Generic;
using System.Text;

namespace E_Wallet.Domain.Entities
{
    public class Bank
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string VerifyUrl { get; set; } = string.Empty;
        public ICollection<AtmOperation> AtmOperations { get; set; } = new List<AtmOperation>();
    }
}
