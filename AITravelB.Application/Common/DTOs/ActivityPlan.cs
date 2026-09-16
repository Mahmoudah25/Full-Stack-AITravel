using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITravelB.Application.Common.DTOs
{
    public class ActivityPlan
    {
        public Guid ActivityId { get; set; }
        public string Time {  get; set; } = string.Empty;
        public string PlaceName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public decimal EstimatedCost { get; set; }
    }
}
