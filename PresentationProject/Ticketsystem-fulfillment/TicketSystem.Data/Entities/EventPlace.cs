using System.ComponentModel.DataAnnotations;

namespace TicketSystem.Data;

public class EventPlace
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = default!;

    [Required]
    public string Address { get; set; } = default!;

    [Required]
    public int Capacity { get; set; } = default!;

    public List<Event> events { get; set; } = default!;
}