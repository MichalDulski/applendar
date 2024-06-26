using Applendar.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Applendar.Infrastructure.Configurations;

internal class EventInvitationConfiguration : IEntityTypeConfiguration<EventInvitation>
{
    public void Configure(EntityTypeBuilder<EventInvitation> builder)
    {
        builder.HasKey(x => new { x.ApplendarUserId, x.EventId });

        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.UpdatedAtUtc).IsRequired();
        builder.Property(x => x.ArchivedAtUtc).IsRequired(false);
    }
}