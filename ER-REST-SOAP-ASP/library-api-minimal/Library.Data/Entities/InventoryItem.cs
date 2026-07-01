namespace Library.Data.Entities;

public class InventoryItem
{
    public int Id { get; set; }
    public int ProductId { get; set; } //FK
    public Product Product { get; set; } = default!;
    public int CurrentStock { get; set; }

    //Adding a RowVersion property - use it in OnModelCreation
    public byte[] RowVersion { get; set; } = default!; //track concurrency.
}