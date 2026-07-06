namespace VideoGameStore.Data.Entities;

public class InventoryItem
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Videogame Videogame { get; set; } = default!;
    public int CurrentStock { get; set; }

    // We will use this to track concurrency.
    public byte[] RowVersion { get; set; } = default!;
}