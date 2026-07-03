using Microsoft.EntityFrameworkCore;
using System.Dynamic;
using System.Data.Common;

namespace TicketSystem.Data;

public class TicketSystemDbContext : DbContext
{
    public TicketSystemDbContext(DbContextOptions<TicketSystemDbContext> options) : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<EventPlace> EventPlaces => Set<EventPlace>();

    public DbSet<EventType> EventTypes => Set<EventType>();

    public DbSet<Event> Events => Set<Event>();

    public DbSet<Ticket> Tickets => Set<Ticket>();

    protected override void OnModelCreating(ModelBuilder b)
    {

    }
}