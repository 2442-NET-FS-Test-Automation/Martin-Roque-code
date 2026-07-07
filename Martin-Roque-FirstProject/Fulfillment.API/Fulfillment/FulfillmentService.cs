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
    public Task<BurstResult> FulfillBurstAsync(IEnumerable<int> buyingIds, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<FulfillmentResult> FulfillOneAsync(int buyingId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}

