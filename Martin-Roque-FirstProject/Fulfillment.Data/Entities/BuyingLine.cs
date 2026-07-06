namespace VideoGameStore.Data.Entities;

public class BuyingLine
{
    public int Id { get; set; }

    public int BuyingId { get; set; }

    public int GameId { get; set; }

    public int Quantity { get; set; }
}