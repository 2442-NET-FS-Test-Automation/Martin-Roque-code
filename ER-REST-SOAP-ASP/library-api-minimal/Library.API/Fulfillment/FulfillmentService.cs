//This class will hold the business logic/db retry logic for fulfilling transactions
using System.Collections;
using Library.Data;
using Library.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Library.API.Fulfillment;

//ASP.NET's builder Needs us to provide 2 things when we register a service. These can both go in the same file

public interface IFulfillmentService
{
    public Task<FulfillmentResult> FulfillOneAsync(int orderId, CancellationToken ct);
    public Task<BurstResult> FulFillBurstAsync(IEnumerable<int> orderIds, CancellationToken ct);
}

public enum FulfillmentResult { Fulfilled, Backordered }

//recors are lightweight custom types that allow for comparison with ==
public record BurstResult(int Fulfilled, int Backordered);

public class FulfillmentService : IFulfillmentService
{
    private readonly IDbContextFactory<LibraryDbContext> _factory;
    private readonly BurstPlanner _planner;

    public FulfillmentService(IDbContextFactory<LibraryDbContext> factory, BurstPlanner planner)
    {
        _factory = factory;
        _planner = planner;
    }

    //Method to handle fulfillment
    public async Task<FulfillmentResult> FulfillOneAsync(int orderId, CancellationToken ct)
    {
        //First - we need a dbcontext
        await using var db = await _factory.CreateDbContextAsync(ct);

        var order = await db.Orders.Include(o => o.Lines).FirstAsync(o => o.Id == orderId, ct);

        //Create dictionary with the ProductId Key and orderId Value
        var requested = order.Lines.ToDictionary(l => l.ProductId, l => l.Quantity);
        //flag for "can i continue fulfilling this order"
        bool canFulfill = true;

        foreach (OrderLine line in order.Lines)
        {
            InventoryItem inv = await db.Inventory.FirstAsync(i => i.ProductId == line.ProductId, ct);

            if (inv.CurrentStock < line.Quantity)
            {
                canFulfill = false;
                break;
            }

            inv.CurrentStock -= line.Quantity;
        }

        if (!canFulfill)
        {
            order.Status = Status.Backordered;

            db.FulfillmentEvents.Add(new FulfillmentEvent { OrderId = orderId, Type = "Backorder" });

            await db.SaveChangesAsync(ct);

            Log.Warning("Backordered {OrderId}: insufficient stock", orderId);

            return FulfillmentResult.Backordered;
        }

        order.Status = Status.Fulfilled;
        order.CompletedUtc = DateTime.UtcNow;
        db.FulfillmentEvents.Add(new FulfillmentEvent { OrderId = orderId, Type = "Fulfilled" });

        //Adding our retry save method
        if (!await SaveWithRetryAsync(db, requested, ct))
        {
            db.ChangeTracker.Clear();
            Order staleOrder = await db.Orders.FirstAsync(o => o.Id == orderId, ct);
            staleOrder.Status = Status.Backordered;
            Log.Warning("Backordered order {OrderId} after concurrency retry", orderId);
            return FulfillmentResult.Backordered;
        }

        Log.Information("Fulfilled order: {OrderId}, {LineCount} lines", orderId, order.Lines.Count);

        return FulfillmentResult.Fulfilled;
    }

    //Lets break the logic for savin with retry (via RowVersion) into its own row version

    public static async Task<bool> SaveWithRetryAsync(
        LibraryDbContext db, IReadOnlyDictionary<int, int> requestedByProductId, CancellationToken ct)
    {
        //Loop forever until we run out of stock
        while (true)
        {
            try
            {
                await db.SaveChangesAsync(ct);
                return true;
            }
            catch (DbUpdateConcurrencyException ex) //How many times to handle the exception for us, +3 ww won't enter the catch
            {
                foreach (var entry in ex.Entries)
                {
                    var current = await entry.GetDatabaseValuesAsync();

                    if (current is null) return false;

                    entry.OriginalValues.SetValues(current);

                    if (entry.Entity is InventoryItem inv)
                    {
                        int freshValue = current.GetValue<int>(nameof(InventoryItem.CurrentStock));
                        //Dictionary lookup against the dict we passed in
                        int desiredAmount = requestedByProductId[inv.ProductId];

                        //Re-check on the refresh stock - don't blindly trust it
                        if (freshValue < desiredAmount) return false; //this is now our exit condition
                        inv.CurrentStock = freshValue - desiredAmount;
                    }
                }
            }
        }

    }

    public async Task<BurstResult> FulFillBurstAsync(IEnumerable<int> orderIds, CancellationToken ct)
    {
        // we are just going to piggyback off of FulfillOneAsync - call it again
        List<int> idList = orderIds.ToList();

        List<Order> orders;

        await using (var db = await _factory.CreateDbContextAsync(ct))
        {
            orders = await db.Orders.Where(o => idList.Contains(o.Id)).ToListAsync();
        }

        var planned = _planner.OrderByPriority(orders);

        var tasks = planned.Select(id => FulfillOneAsync(id, ct)); //each call will get its own dbContext

        //Await here until all tasks in the collection are complete
        var results = await Task.WhenAll(tasks);

        return new BurstResult(
            Fulfilled: results.Count(r => r == FulfillmentResult.Fulfilled),
            Backordered: results.Count(r => r == FulfillmentResult.Backordered)
        );
    }
}