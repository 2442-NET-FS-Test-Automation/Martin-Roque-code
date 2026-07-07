namespace VideoGameStore.Data.Entities;

public class Buying
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public Customer Customer { get; set; } = default!;

    public Priority Priority { get; set; } = default!;

    public Status Stauts { get; set; } = default!;

    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    public DateTime? CompletedUtc { get; set; }

    public List<BuyingLine> Lines { get; set; } = new();
}