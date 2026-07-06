namespace VideoGameStore.Data.Entities;

public class FulfillmentEvent
{
    public int Id { get; set; }
    public int BuyingId { get; set; }
    public string Type { get; set; } = default!;
    public DateTime FulfilledAtUtc { get; set; } = DateTime.UtcNow;
}