using AITravelB.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITravelB.Application.Ratings.Commands.CreateRating
{
    public class CreateRatingCommandHandler : IRequestHandler<CreateRatingCommand, Guid>
    {
        private readonly IApplicationDbContext context;
        public CreateRatingCommandHandler(IApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<Guid> Handle(CreateRatingCommand request, CancellationToken cancellationToken)
        {
            var activity = await context.Activities
                .Include(c => c.Trip)
                .FirstOrDefaultAsync(c => c.Id == request.ActivityId, cancellationToken);
            Console.WriteLine(request.ActivityId);
            Console.WriteLine(await context.Activities.CountAsync());

            if (activity == null)
                throw new Exception("Activity not found");
            var stratdate = activity.Trip.StartDate.AddDays(activity.Trip.Days);
            if (stratdate > DateTime.UtcNow)
                throw new Exception("You can only rate an activity after the trip has ended.");
            var rating = new Domain.Entities.Rating
            (
                request.ActivityId,
                request.CustomerEmail,
                request.Score,
                request.Comment
            );
            await context.Ratings.AddAsync(rating, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            return rating.Id;

        }
    }
}
