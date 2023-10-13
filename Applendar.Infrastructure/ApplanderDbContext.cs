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
}