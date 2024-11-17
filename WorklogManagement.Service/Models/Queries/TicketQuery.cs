using WorklogManagement.Service.Enums;

namespace WorklogManagement.Service.Models.Queries;

public class TicketQuery : Query
{
    public int? RefId { get; set; }

    public string? Title { get; set; }

    public string? Search { get; set; }

    public IEnumerable<TicketStatus>? Status { get; set; }
}
