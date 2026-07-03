using System.ComponentModel.DataAnnotations;

namespace TicketSystem.Data;

public class Event
{
    public int Id { get; set; }
    public int EventTypeId { get; set; }

    public EventType? eventType { get; set; }

    public int EventPlaceId { get; set; }
    public EventPlace? eventPlace { get; set; }

    [Required]
    public string Name { get; set; } = default!;

    [Required]
    public int Seats { get; set; } = default!;

    [Required]
    public int VIPSeats { get; set; } = default!;

    public List<Ticket> tickets { get; set; } = new();
}