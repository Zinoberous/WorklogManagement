namespace WorklogManagement.API.Models.Filter
{
    public partial class WorklogFilter : IFilter
    {
        public int? DayId { get; set; }

        public int? TicketId { get; set; }
    }
}
