using Applendar.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Applendar.Infrastructure.Configurations;

internal class ApplendarUserConfiguration : IEntityTypeConfiguration<ApplendarUser>
{
    public void Configure(EntityTypeBuilder<ApplendarUser> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.FirstName).IsRequired();
        builder.Property(x => x.LastName).IsRequired();
        builder.Property(x => x.ExternalId).IsRequired();

        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.UpdatedAtUtc).IsRequired();
        builder.Property(x => x.ArchivedAtUtc).IsRequired(false);

        builder.OwnsOne(x => x.Preferences);

        builder.HasMany(x => x.EventInvitations)
            .WithOne(x => x.ApplendarUser)
            .HasForeignKey(x => x.ApplendarUserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}