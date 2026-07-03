using System.ComponentModel.DataAnnotations;

namespace TicketSystem.Data;

public class EventType
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = default!;

    [Required]
    public string Description { get; set; } = default!;

    public List<Event> events { get; set; } = new();
}