using Applander.Domain.Common;
using Applander.Domain.Entities;
using Applander.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Applander.Infrastructure;

public class ApplendarDbContext : DbContext
{
    public ApplendarDbContext(DbContextOptions<ApplendarDbContext> options) : base(options) { }

    public DbSet<ApplendarUser> ApplendarUsers { get; set; }
    public DbSet<EventInvitation> EventInvitations { get; set; }
    public DbSet<Event> Events { get; set; }

    public override int SaveChanges()
    {
        AddTimestamps();

        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AddTimestamps();

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new EventConfiguration());
        modelBuilder.ApplyConfiguration(new ApplendarUserConfiguration());
        modelBuilder.ApplyConfiguration(new EventInvitationConfiguration());
    }

    // https://stackoverflow.com/questions/45429719/automatic-createdat-and-updatedat-fields-onmodelcreating-in-ef6
    private void AddTimestamps()
    {
        IEnumerable<EntityEntry> entities = ChangeTracker.Entries()
            .Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));

        foreach (EntityEntry entity in entities)
        {
            DateTime now = DateTime.UtcNow; // current datetime

            if (entity.State == EntityState.Added)
                ((BaseEntity)entity.Entity).CreatedAtUtc = now;

            ((BaseEntity)entity.Entity).UpdatedAtUtc = now;
        }
    }
}