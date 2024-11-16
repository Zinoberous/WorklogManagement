using DB = WorklogManagement.Data.Models;

namespace WorklogManagement.Service.Models;

public class TicketStatus
{
    public required int Id { get; init; }

    public required string Name { get; init; }

    internal static TicketStatus Map(DB.TicketStatus status)
    {
        return new()
        {
            Id = status.Id,
            Name = status.Name,
        };
    }
}
