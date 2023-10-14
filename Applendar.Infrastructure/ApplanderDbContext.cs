using Applander.Domain.Common;
using Applander.Domain.Entities;
using Applander.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Applander.Infrastructure;

public class ApplanderDbContext : DbContext
{
    public DbSet<Event> Event { get; set; }

    public ApplanderDbContext(DbContextOptions<ApplanderDbContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new EventConfiguration());
    }
    
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

    // https://stackoverflow.com/questions/45429719/automatic-createdat-and-updatedat-fields-onmodelcreating-in-ef6
    private void AddTimestamps()
    {
        var entities = ChangeTracker.Entries()
            .Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));

        foreach (var entity in entities)
        {
            var now = DateTime.UtcNow; // current datetime

            if (entity.State == EntityState.Added)
            {
                ((BaseEntity)entity.Entity).CreatedAtUtc = now;
            }
            ((BaseEntity)entity.Entity).UpdatedAtUtc = now;
        }
    }
}