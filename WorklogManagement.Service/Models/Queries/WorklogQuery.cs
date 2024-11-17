namespace WorklogManagement.Service.Models.Queries;

public class WorklogQuery : Query
{
    public DateOnly? Date { get; set; }

    public int? TicketId { get; set; }

    public string? Search { get; set; }
}
