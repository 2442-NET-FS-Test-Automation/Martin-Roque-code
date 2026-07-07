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
    public static readonly string[] specialIds = { "VGM-001", "VGM-002", "VGM-003", "VGM-004" };

    private readonly IDbContextFactory<VideoGameStoreDbContext> _factory;

    public Seeder(IDbContextFactory<VideoGameStoreDbContext> factory)
    {
        _factory = factory;
    }

    public IReadOnlyList<int> SeedBuyings(int n, bool expedited)
    {
        using var db = _factory.CreateDbContext();

        var vid = db.Videogames.ToDictionary(v => v.SpeIden, v => v.Id);

        var ids = new List<int>(n);

        for (int i = 0; i < n; i++)
        {
            var buying = new Buying
            {
                CustomerId = Random.Shared.Next(1, 3),
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
                CustomerId = Random.Shared.Next(1, 3),
                Priority = i % 3 == 0 ? Priority.Expedited : Priority.Normal,
                Lines = { new BuyingLine { GameId = vid[new[] { "VGM-001", "VGM-002", "VGM-003", "VGM-004" }[i % 3]], Quantity = 1 } }
            };

            db.Buyings.Add(buying);
            db.SaveChanges();
            ids.Add(buying.Id);
        }
        return ids;

    }
}