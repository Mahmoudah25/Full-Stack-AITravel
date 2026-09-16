using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITravelB.Application.Common.DTOs
{
    public class ItineraryResult
    {
        public List<DayPlan> Days { get; set; } = new List<DayPlan>();
        public decimal TotalEstimatedCost { get; set; }

    }
}
