using AITravelB.Domain.Enum;
using AITravelB.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITravelB.Domain.Entities
{
    public class Trip
    {
        public Guid Id { get; private set; }
        public string Destination { get; private set; } = string.Empty;
        public DateTime StartDate { get; private set; }
        public int Days { get; private set; } 
     
        public TripStatus status { get; private set; } = TripStatus.Draft;
        public Budget Budget { get; set; }
        private readonly List<Activity> activities = new List<Activity>();
        public IReadOnlyCollection<Activity> Activities => activities.AsReadOnly();

        private Trip() { }
        public Trip(string destination, DateTime startDate, int days, Budget budget)
        {
            if(string.IsNullOrWhiteSpace(destination))
            {
                throw new ArgumentException("Destination cannot be null or empty.");
            }
            if(days <= 0)
            {
                throw new ArgumentException("Days must be greater than zero.");
            }
            Id = Guid.NewGuid();
            Destination = destination;
            StartDate = startDate;
            Days = days;
            Budget = budget;
        }

        public void AddActivity(Activity activity)
        {
            if (activity == null)
            {
                throw new ArgumentNullException(nameof(activity), "Activity cannot be null.");
            }
            activities.Add(activity);
        }

    }
}
