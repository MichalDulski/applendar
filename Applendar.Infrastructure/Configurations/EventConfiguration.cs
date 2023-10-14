using Applander.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Applander.Infrastructure.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(255);
        builder.Property(x => x.MaximumNumberOfParticipants).IsRequired(false);
        builder.Property(x => x.IsCompanionAllowed).IsRequired();
        builder.Property(x => x.IsPetAllowed).IsRequired();
        builder.Property(x => x.StartAtUtc).IsRequired();
        builder.Property(x => x.IsCompanionAllowed).IsRequired();
        builder.Property(x => x.IsPetAllowed).IsRequired();
        builder.Property(x => x.Image).IsRequired(false);
        builder.Property(x => x.EventType).IsRequired();
        
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.UpdatedAtUtc).IsRequired();
        builder.Property(x => x.ArchivedAtUtc).IsRequired(false);

        builder.OwnsOne(e => e.Location);

        builder.HasOne(x => x.Organizer)
            .WithMany(x => x.OrganizedEvents)
            .HasForeignKey(x => x.OrganizerId)
            .IsRequired(true);

        builder.HasMany(x => x.Invitations)
            .WithOne(x => x.Event)
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}