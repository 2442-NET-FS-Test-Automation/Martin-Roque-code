using Microsoft.EntityFrameworkCore;
using Library.Data;
using Library.Data.Entities;
using System.Dynamic;
using Microsoft.Identity.Client;

namespace Library.Data;

public class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<InventoryItem> Inventory => Set<InventoryItem>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderLine> OrderLines => Set<OrderLine>();
    public DbSet<FulfillmentEvent> FulfillmentEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder b)
    {
        //Three ways Convention, Data Annotations, Fluent API
        b.Entity<Product>(e =>
        {
            //Set an index
            e.HasIndex(p => p.Sku).IsUnique();
            //Setting decimal places
            e.Property(p => p.Price).HasColumnType("decimal(10,2)");
            //relationship
            e.HasOne(p => p.Inventory)
                    .WithOne(i => i.Product)
                    .HasForeignKey<InventoryItem>(i => i.ProductId);
        });

        b.Entity<InventoryItem>().Property(i => i.RowVersion).IsRowVersion();

        b.Entity<Customer>().Property(c => c.Email).HasMaxLength(256);
        b.Entity<Customer>().HasIndex(c => c.Email).IsUnique();

        b.Entity<Product>().HasData(
            new Product { Id = 1, Sku = "BK-001", Name = "Clean Code", Price = 32.00m },
            new Product { Id = 2, Sku = "BK-002", Name = "The Pragmatic Programmer", Price = 38.00m },
            new Product { Id = 3, Sku = "BK-003", Name = "Refactoring", Price = 45.00m }
        );

        b.Entity<InventoryItem>().HasData(
            new InventoryItem { Id = 1, ProductId = 1, CurrentStock = 5 },
            new InventoryItem { Id = 2, ProductId = 2, CurrentStock = 3 },
            new InventoryItem { Id = 3, ProductId = 3, CurrentStock = 8 }
        );
        b.Entity<Customer>().HasData(
            new Customer { Id = 1, Name = "Ada Lovelance", Email = "ada@exaple.com" },
            new Customer { Id = 2, Name = "Alan Turing ", Email = "alan@example.com" }
        );
    }
}