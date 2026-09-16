using AITravelB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITravelB.Persistence.Configrations
{
    public class RatingConfiguration : IEntityTypeConfiguration<Rating>
    {
        public void Configure(EntityTypeBuilder<Rating> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(m => m.CustomerEmail)
                .IsRequired()
                .HasMaxLength(200);
            builder.HasIndex(r=> new
            {
                r.ActivityId,
                r.CustomerEmail
            })
            .IsUnique();
            builder.Property(p => p.Score)
                .IsRequired();
            builder.Property(p => p.Comment)
                .HasMaxLength(1000);
            builder.HasOne<Activity>()
                .WithMany(a => a.Ratings)
                .HasForeignKey(r => r.ActivityId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
