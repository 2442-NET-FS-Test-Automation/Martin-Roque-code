using VideoGameStore.Data;
using VideoGameStore.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace VideoGameStore.API.Fulfillment;

public interface IFulfillmentService
{
    public Task<FulfillmentResult> FulfillOneAsync(int buyingId, CancellationToken ct);
    public Task<BurstResult> FulfillBurstAsync(IEnumerable<int> buyingIds, CancellationToken ct);
}

public enum FulfillmentResult { Fulfilled, Backordered }

public record BurstResult(int Fulfilled, int Backordered);

public class FulfillmentService : IFulfillmentService
{
    private readonly IDbContextFactory<VideoGameStoreDbContext> _factory;
    private readonly BurstPlanner _planner;

    public FulfillmentService(IDbContextFactory<VideoGameStoreDbContext> factory, BurstPlanner planner)
    {
        _factory = factory;
        _planner = planner;
    }
    public async Task<FulfillmentResult> FulfillOneAsync(int buyingId, CancellationToken ct)
    {
        // First - we need a db context 
        await using var db = await _factory.CreateDbContextAsync(ct);

        // Lets grab our order from the database
        // FLow for this - a customer places an order. It hits the order table - we are now fulfilling that order
        var buying = await db.Buyings.Include(o => o.Lines).FirstAsync(o => o.Id == buyingId, ct); // LINQ with Async

        // Lets create that dictionary with the productId Key and the OrderId value
        // yay for LINQ/Collections namespace
        var requested = buying.Lines.ToDictionary(l => l.GameId, l => l.Quantity);

        // creating a flag for "can i continue fulfilling this order"
        bool canFulfill = true;

        foreach (BuyingLine line in buying.Lines)
        {
            // First - grab the current inventory from the db for that product
            InventoryItem inv = await db.Inventory.FirstAsync(i => i.ProductId == line.GameId);

            // Next - check if we can meet the order 
            if (inv.CurrentStock < line.Quantity)
            {
                canFulfill = false;
                break;
            }

            inv.CurrentStock -= line.Quantity; // This write to the InventoryItem table is guarded by RowVersion
        }

        // assuming we broke out of the foreach and cannot fulfill the order
        if (!canFulfill) // checking for canFulfill == false
        {
            // We can't fulfill this order, its now Backordered
            buying.Stauts = Status.Backordered;

            // Create a new fulfillment event record for this transaction, setting it to backordered.
            db.FulfillmentEvents.Add(new FulfillmentEvent { BuyingId = buyingId, Type = "Backorder" });

            await db.SaveChangesAsync(ct);

            // Log the transaction, using the Serilog structured logging syntax
            Log.Warning("Backordered {BuyingId}: insufficient stock", buyingId);

            return FulfillmentResult.Backordered;
        }

        // If we make it here, we CAN fulfill that order
        buying.Stauts = Status.Fulfilled;
        buying.CompletedUtc = DateTime.UtcNow;
        db.FulfillmentEvents.Add(new FulfillmentEvent { BuyingId = buyingId, Type = "Fulfilled" });


        if (!await SaveWithRetryAsync(db, requested, ct))
        {// that stock dropped this order was backordered
            db.ChangeTracker.Clear(); // clear change tracker
            Buying staleBuying = await db.Buyings.FirstAsync(o => o.Id == buyingId, ct); //grab stale order from db
            staleBuying.Stauts = Status.Backordered; // set its status to backordered
            Log.Warning("Backordered order {BuyingId} after concurrency retry", buyingId);
            return FulfillmentResult.Backordered;
        }

        Log.Information("Fulfilled order: {BuyingId}, {LineCount} lines", buyingId, buying.Lines.Count);
        return FulfillmentResult.Fulfilled;
    }

    // Lets break the logic for saving with retry (via RowVersion) into its own method
    // just to help keep things straight. IReadOnlyDictionary just makes any dict we pass in readonly
    private static async Task<bool> SaveWithRetryAsync(
        VideoGameStoreDbContext db, IReadOnlyDictionary<int, int> requestedByProductId, CancellationToken ct)
    {

        // This is that RowVersion Change Tracker entry retry from yesterday
        // NEW: Loop forever until we run out of stock
        while (true)
        {

            // Our loop as written never exits - it does increment attempt for us.
            // If we retry and fail x amount of times - we will throw an exception manually
            try
            {
                // The DbContext inside this method came from FulfillOneAsync - if it has changes 
                // staged to it - we can save them here. Its the same object.
                await db.SaveChangesAsync(ct);
                return true;
            }
            // We can tell our try catch how many times to handle this exception for us
            // After 3 attempts - we won't enter the catch. It bubbles up to wherever this method 
            // was called
            catch (DbUpdateConcurrencyException ex)
            {
                Log.Warning("Attempty retry");

                // Retry logic - remember that Change Tracker stuff?
                // entry is an EF Core Change tracker entry
                foreach (var entry in ex.Entries)
                {

                    var current = await entry.GetDatabaseValuesAsync(); // grab the current database values

                    // Is some other user deleted the entry out from under us... we can't save
                    // return false
                    if (current is null) return false;

                    // Set the OriginalValues bucket on the entry to what they currently are
                    entry.OriginalValues.SetValues(current);

                    if (entry.Entity is InventoryItem inv)
                    {
                        // Grab the current totals for that item's stock
                        int freshValue = current.GetValue<int>(nameof(InventoryItem.CurrentStock));
                        //Dictionary lookup against the dict we passed in
                        int desiredAmount = requestedByProductId[inv.ProductId];

                        // Re-check on the fresh stock - don't blindly trust it
                        if (freshValue < desiredAmount) return false; // this is now our exit condition
                        inv.CurrentStock = freshValue - desiredAmount;
                    }
                }
            }
        }
    }

    public async Task<BurstResult> FulfillBurstAsync(IEnumerable<int> buyingIds, CancellationToken ct)
    {
        // Grabbing all my orderIds
        List<int> idList = buyingIds.ToList();

        List<Buying> buyings; // place to store my orders

        // Calling on a dbcontext that we discard after we're done
        await using (var db = await _factory.CreateDbContextAsync(ct))
        {   // Look in our db, grab every order that appears in our idList
            buyings = await db.Buyings.Where(b => idList.Contains(b.Id)).ToListAsync();
        }

        // Calling on our planning logic inside BurstPlanner
        // planned contains our expedited/priority first order
        var planned = _planner.BuyingByPriority(buyings);

        // we are just going to piggyback off of FulfilOneAsync - no need to rewrite logic we can just call it again
        var tasks = planned.Select(id => FulfillOneAsync(id, ct)); // each call will get its own dbContext

        // Await here until all tasks in the collection are complete
        var results = await Task.WhenAll(tasks);

        return new BurstResult(
            Fulfilled: results.Count(r => r == FulfillmentResult.Fulfilled),
            Backordered: results.Count(r => r == FulfillmentResult.Backordered)
        );

    }


}

