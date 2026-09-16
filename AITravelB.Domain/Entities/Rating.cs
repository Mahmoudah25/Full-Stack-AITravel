using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITravelB.Domain.Entities
{
    public class Rating
    {
        public Guid Id { private set; get; }    
        public Guid ActivityId { private set; get; }
        public string CustomerEmail { private set; get; }   =string.Empty;
        public int Score { private set; get; }  
        public string? Comment { private set; get; }
        public DateTime CreatedAt { private set; get; } = DateTime.UtcNow;
        private Rating() { }    
        public Rating(Guid activityId, string customerEmail, int score, string? comment = null)
        {
            if(activityId == Guid.Empty)
                throw new ArgumentException("ActivityId cannot be empty.", nameof(activityId));
            if (string.IsNullOrWhiteSpace(customerEmail))
                throw new ArgumentException("CustomerEmail cannot be null or empty.", nameof(customerEmail));
            if (score < 1 || score > 5)
                throw new ArgumentOutOfRangeException(nameof(score), "Score must be between 1 and 5.");
            Id = Guid.NewGuid();
            ActivityId = activityId;
            CustomerEmail = customerEmail;
            Score = score;
            Comment = comment;
        }

    }
}
