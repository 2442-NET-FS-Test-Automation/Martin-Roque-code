using VideoGameStore.Data;
using VideoGameStore.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace VideoGameStore.API.Fulfillment;

//We put all fulfillment logic inside the same file just to make presentation more concrete
//Normally we create a file to interface, service, enum and record
public interface IFulfillmentService
{
    public Task<FulfillmentResult> FulfillOneAsync(int buyingId, CancellationToken ct);
    public Task<BurstResult> FulfillBurstAsync(IEnumerable<int> buyingIds, CancellationToken ct);
}

//Defining how a buying can be solved
public enum FulfillmentResult { Fulfilled, Backordered }

//Linking the enum to a record to manage each buying in the burst method
public record BurstResult(int Fulfilled, int Backordered);

public class FulfillmentService : IFulfillmentService
{
    //Declarin all the services the system need to this process 
    private readonly IDbContextFactory<VideoGameStoreDbContext> _factory;
    private readonly BurstPlanner _planner;

    //Injecting all dependencies
    public FulfillmentService(IDbContextFactory<VideoGameStoreDbContext> factory, BurstPlanner planner)
    {
        _factory = factory;
        _planner = planner;
    }

    //Core method to fulfill a buying in this application
    public async Task<FulfillmentResult> FulfillOneAsync(int buyingId, CancellationToken ct)
    {
        //We need a db context 
        await using var db = await _factory.CreateDbContextAsync(ct);

        // Lets grab our order from the database
        //FLow for this -> a customer places an order. It hits the order table -> we are now fulfilling that order
        var buying = await db.Buyings.Include(o => o.Lines).FirstAsync(o => o.Id == buyingId, ct);

        //Creating that dictionary with the productId Key and the OrderId value
        var requested = buying.Lines.ToDictionary(l => l.GameId, l => l.Quantity);

        //Creating a flag to know when we can still fulfill buyings
        bool canFulfill = true;

        foreach (BuyingLine line in buying.Lines)
        {
            //Grab the current inventory from the db for that product
            InventoryItem inv = await db.Inventory.FirstAsync(i => i.ProductId == line.GameId);

            //Check if it can meet the order 
            if (inv.CurrentStock < line.Quantity)
            {
                canFulfill = false;
                break;
            }

            inv.CurrentStock -= line.Quantity; //This write to the InventoryItem table is guarded by RowVersion
        }

        //Assuming it broke out of the foreach and cannot fulfill the order
        if (!canFulfill)
        {
            //It can't fulfill this order, its now Backordered
            buying.Stauts = Status.Backordered;

            //Create a new fulfillment event record for this transaction, setting it to backordered.
            db.FulfillmentEvents.Add(new FulfillmentEvent { BuyingId = buyingId, Type = "Backorder" });

            await db.SaveChangesAsync(ct);

            //Log the transaction, using the Serilog structured logging syntax
            Log.Warning("Backordered {BuyingId}: insufficient stock", buyingId);

            return FulfillmentResult.Backordered;
        }

        //If it is here, it can fulfill that order
        buying.Stauts = Status.Fulfilled;
        buying.CompletedUtc = DateTime.UtcNow;
        db.FulfillmentEvents.Add(new FulfillmentEvent { BuyingId = buyingId, Type = "Fulfilled" });


        if (!await SaveWithRetryAsync(db, requested, ct))
        {
            db.ChangeTracker.Clear(); //Clear change tracker
            Buying staleBuying = await db.Buyings.FirstAsync(o => o.Id == buyingId, ct);
            staleBuying.Stauts = Status.Backordered; //Set buyings status to backordered
            Log.Warning("Backordered order {BuyingId} after concurrency retry", buyingId);
            return FulfillmentResult.Backordered;
        }

        Log.Information("Fulfilled order: {BuyingId}, {LineCount} lines", buyingId, buying.Lines.Count);
        return FulfillmentResult.Fulfilled;
    }
    private static async Task<bool> SaveWithRetryAsync(
        VideoGameStoreDbContext db, IReadOnlyDictionary<int, int> requestedByProductId, CancellationToken ct)
    {
        //This is that RowVersion Change Tracker
        //Loop forever until we run out of stock
        while (true)
        {

            // Our loop as written never exits - it does increment attempt for us.
            // If we retry and fail x amount of times - we will throw an exception manually
            try
            {
                //The DbContext inside this method came from FulfillOneAsync - if it has changes 
                //staged to it - it can save them here. Its the same object.
                await db.SaveChangesAsync(ct);
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Log.Warning("Attempty retry");

                //Retry logic
                foreach (var entry in ex.Entries)
                {

                    var current = await entry.GetDatabaseValuesAsync();

                    if (current is null) return false;

                    entry.OriginalValues.SetValues(current);

                    if (entry.Entity is InventoryItem inv)
                    {
                        int freshValue = current.GetValue<int>(nameof(InventoryItem.CurrentStock));

                        int desiredAmount = requestedByProductId[inv.ProductId];

                        //Re-check on the fresh stock - don't blindly trust it
                        if (freshValue < desiredAmount) return false;
                        inv.CurrentStock = freshValue - desiredAmount;
                    }
                }
            }
        }
    }

    public async Task<BurstResult> FulfillBurstAsync(IEnumerable<int> buyingIds, CancellationToken ct)
    {
        //Grabbing all my orderIds
        List<int> idList = buyingIds.ToList();

        List<Buying> buyings;

        //Calling on a dbcontext that we discard after we're done
        await using (var db = await _factory.CreateDbContextAsync(ct))
        {
            buyings = await db.Buyings.Where(b => idList.Contains(b.Id)).ToListAsync();
        }

        //Calling our burstplanner
        var planned = _planner.BuyingByPriority(buyings);

        //Repeting the logic to fulfill one buying - repeting task
        var tasks = planned.Select(id => FulfillOneAsync(id, ct));

        //Await here until all tasks in the collection are complete
        var results = await Task.WhenAll(tasks);

        return new BurstResult(
            Fulfilled: results.Count(r => r == FulfillmentResult.Fulfilled),
            Backordered: results.Count(r => r == FulfillmentResult.Backordered)
        );

    }
}

