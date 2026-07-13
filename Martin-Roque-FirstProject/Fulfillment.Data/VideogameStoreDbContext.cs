using Microsoft.EntityFrameworkCore;
using VideoGameStore.Data.Entities;

namespace VideoGameStore.Data;

public class VideoGameStoreDbContext : DbContext
{
    //Creating a constructor to use dbContext in the application
    public VideoGameStoreDbContext(DbContextOptions<VideoGameStoreDbContext> options) : base(options) { }

    //Creating the tables in database and inside application scope
    public DbSet<Videogame> Videogames => Set<Videogame>();
    public DbSet<InventoryItem> Inventory => Set<InventoryItem>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Buying> Buyings => Set<Buying>();
    public DbSet<BuyingLine> BuyingLines => Set<BuyingLine>();
    public DbSet<FulfillmentEvent> FulfillmentEvents { get; set; }

    //This override let us create constraints (sometimes this override the entity file)
    //some constraint will be repeat just to avoid some mixup in the tables
    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Videogame>(e =>
        {
            e.HasIndex(p => p.SpeIden).IsUnique(); //Unique value constraint
            e.Property(p => p.Price).HasColumnType("decimal(10,2)");

            e.HasOne(p => p.Inventory)
                .WithOne(i => i.Videogame)
                .HasForeignKey<InventoryItem>(i => i.ProductId);
        });

        mb.Entity<InventoryItem>().Property(i => i.RowVersion).IsRowVersion(); //Make sure sql server we take this to look the version

        mb.Entity<Customer>().Property(c => c.Email).HasMaxLength(256);
        mb.Entity<Customer>().HasIndex(c => c.Email).IsUnique();

        //Default data we will use to this project (anyone can recreate the same behavior if have sqlserver, visual studio code and sa credentials)
        mb.Entity<Videogame>().HasData(
            new Videogame { Id = 1, SpeIden = "VGM-001", Name = "Donkey Kong Bananza", ESRB = "E for Everyone", Price = 78.50m },
            new Videogame { Id = 2, SpeIden = "VGM-002", Name = "Halo Remaster Collection", ESRB = "+13 years old", Price = 48.90m },
            new Videogame { Id = 3, SpeIden = "VGM-003", Name = "Minecraft", ESRB = "E for Everyone", Price = 22.75m },
            new Videogame { Id = 4, SpeIden = "VGM-004", Name = "Terraria", ESRB = "E+10", Price = 15.50m },
            new Videogame { Id = 5, SpeIden = "VGM-005", Name = "Splatoon Riders", ESRB = "E+10", Price = 65.50m },
            new Videogame { Id = 6, SpeIden = "VGM-006", Name = "GTA VI", ESRB = "M+18", Price = 99.99m }
        );

        mb.Entity<InventoryItem>().HasData(
            new InventoryItem { Id = 1, ProductId = 1, CurrentStock = 5 },
            new InventoryItem { Id = 2, ProductId = 2, CurrentStock = 3 },
            new InventoryItem { Id = 3, ProductId = 3, CurrentStock = 9 },
            new InventoryItem { Id = 4, ProductId = 4, CurrentStock = 7 },
            new InventoryItem { Id = 5, ProductId = 5, CurrentStock = 4 },
            new InventoryItem { Id = 6, ProductId = 6, CurrentStock = 11 }
        );

        mb.Entity<Customer>().HasData(
            new Customer { Id = 1, Name = "Lyon Kennedy", Email = "lyon.kennedy@gmail.com" },
            new Customer { Id = 2, Name = "Lara Croft", Email = "lara.croft@gmail.com" },
            new Customer { Id = 3, Name = "Fox McCloud", Email = "fox.mccloud@gmail.com" },
            new Customer { Id = 4, Name = "Axel Blaze", Email = "axel.blaze@gmail.com" },
            new Customer { Id = 5, Name = "Nene Kusanagi", Email = "nene.kusanagu@gmail.com " }
        );
    }
}