using VideoGameStore.Data;
using VideoGameStore.Data.Entities;
using Microsoft.EntityFrameworkCore;

public interface ISeeder
{
    IReadOnlyList<int> SeedBuyings(int n, bool expedited);
    IReadOnlyList<int> ResetAndCreateBuyings(int n);
}

public class Seeder : ISeeder
{
    //Specify all possible videogames that exists in this project
    public static readonly string[] specialIds = { "VGM-001", "VGM-002", "VGM-003", "VGM-004", "VGM-005", "VGM-006" };

    private readonly IDbContextFactory<VideoGameStoreDbContext> _factory;

    public Seeder(IDbContextFactory<VideoGameStoreDbContext> factory)
    {
        _factory = factory;
    }

    //Method used for restablish inventory stock to default values
    public IReadOnlyList<int> SeedBuyings(int n, bool expedited)
    {
        using var db = _factory.CreateDbContext();

        var vid = db.Videogames.ToDictionary(v => v.SpeIden, v => v.Id);

        var ids = new List<int>(n);

        for (int i = 0; i < n; i++)
        {
            var buying = new Buying
            {
                CustomerId = Random.Shared.Next(1, 6),
                Priority = expedited ? Priority.Expedited : Priority.Normal,
                Lines = { new BuyingLine { GameId = vid[specialIds[i % specialIds.Length]], Quantity = 1 } }
            };

            db.Buyings.Add(buying);
            db.SaveChanges();
            ids.Add(buying.Id);
        }
        return ids;
    }

    public IReadOnlyList<int> ResetAndCreateBuyings(int n)
    {
        using var db = _factory.CreateDbContext();

        foreach (InventoryItem inv in db.Inventory)
        {
            switch (inv.ProductId)
            {
                case 1:
                    inv.CurrentStock = 5;
                    break;
                case 2:
                    inv.CurrentStock = 3;
                    break;
                case 3:
                    inv.CurrentStock = 9;
                    break;
                case 4:
                    inv.CurrentStock = 7;
                    break;
                case 5:
                    inv.CurrentStock = 4;
                    break;
                case 6:
                    inv.CurrentStock = 11;
                    break;
                default:
                    break;
            }

        }
        db.SaveChanges();

        var vid = db.Videogames.ToDictionary(v => v.SpeIden, v => v.Id);

        var ids = new List<int>(n);

        for (int i = 0; i < n; i++)
        {
            var buying = new Buying
            {
                CustomerId = Random.Shared.Next(1, 6),
                Priority = i % 3 == 0 ? Priority.Expedited : Priority.Normal,
                Lines = { new BuyingLine { GameId = vid[new[] { "VGM-001", "VGM-002", "VGM-003", "VGM-004", "VGM-005", "VGM-006" }[i % specialIds.Length]], Quantity = 1 } }
            };

            db.Buyings.Add(buying);
            db.SaveChanges();
            ids.Add(buying.Id);
        }
        return ids;

    }
}