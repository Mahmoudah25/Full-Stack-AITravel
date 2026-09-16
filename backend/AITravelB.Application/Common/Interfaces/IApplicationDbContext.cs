using AITravelB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITravelB.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
            DbSet<Booking> Bookings { get; }    
            DbSet<Trip> Trips { get; }
            DbSet<Activity> Activities { get; }
            DbSet<Rating> Ratings { get; }  
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
