using WorklogManagement.Shared.Enums;

namespace WorklogManagement.UI.Services;

public interface ITicketStatusService
{
    IEnumerable<TicketStatus> GetNextStatusOptions(TicketStatus ticketStatus);
}

public class TicketStatusService : ITicketStatusService
{
    public IEnumerable<TicketStatus> GetNextStatusOptions(TicketStatus ticketStatus)
    {
        return ticketStatus switch
        {
            TicketStatus.Todo => [
                TicketStatus.Todo,
                    TicketStatus.Running,
                    TicketStatus.Blocked,
                    TicketStatus.Canceled,
                    TicketStatus.Continuous
            ],
            TicketStatus.Running => [
                TicketStatus.Running,
                    TicketStatus.Todo,
                    TicketStatus.Paused,
                    TicketStatus.Blocked,
                    TicketStatus.Done,
                    TicketStatus.Canceled,
                    TicketStatus.Continuous
            ],
            TicketStatus.Paused => [
                TicketStatus.Paused,
                    TicketStatus.Running,
                    TicketStatus.Blocked,
                    TicketStatus.Canceled,
                    TicketStatus.Continuous
            ],
            TicketStatus.Blocked => [
                TicketStatus.Blocked,
                    TicketStatus.Todo,
                    TicketStatus.Running,
                    TicketStatus.Paused,
                    TicketStatus.Canceled,
                    TicketStatus.Continuous
            ],
            TicketStatus.Done => [
                TicketStatus.Done,
                    TicketStatus.Todo,
                    TicketStatus.Running
            ],
            TicketStatus.Canceled => [
                TicketStatus.Canceled,
                    TicketStatus.Todo,
                    TicketStatus.Running
            ],
            TicketStatus.Continuous => [
                TicketStatus.Continuous,
                    TicketStatus.Todo,
                    TicketStatus.Running,
                    TicketStatus.Done,
                    TicketStatus.Canceled
            ],
            _ => []
        };
    }
}
