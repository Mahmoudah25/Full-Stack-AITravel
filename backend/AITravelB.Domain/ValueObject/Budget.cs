using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITravelB.Domain.ValueObject
{
    public record Budget
    {
        public decimal Amount { get; }
        public string Currency { get; }
        public Budget(decimal amount, string currency)
        {
            if (amount < 0)
            {
                throw new ArgumentException("Amount must be greater than or equal to 0.");
            }
            if (string.IsNullOrWhiteSpace(currency))
            {
                throw new ArgumentException("Currency must not be empty.");
            }
            Amount = amount;
            Currency = currency;
        }

    }
}
