using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITravelB.Application.Trips.Queries.GrtTripById
{
    public class TripDto
    {
        public Guid Id { get; set; }
        public string Destination { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public int Days { get; set; }
        public decimal BudgetAmount { get; set; }
        public string BudgetCurrency { get; set; } = string.Empty;  
    }
}
