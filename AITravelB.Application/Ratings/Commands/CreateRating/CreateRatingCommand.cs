using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITravelB.Application.Ratings.Commands.CreateRating
{
    public class CreateRatingCommand : IRequest<Guid>
    {
        public Guid ActivityId {  set; get; }
        public string CustomerEmail { set; get; } = string.Empty;
        public int Score {  set; get; }
        public string? Comment {  set; get; }

    }
}
