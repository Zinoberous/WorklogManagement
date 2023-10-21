using System.Text.Json.Serialization;

namespace WorklogManagement.API.Models.Filter
{
    public partial class WorklogFilter : IFilter
    {
        [JsonPropertyName("dayId")]
        public int? DayId { get; set; }

        [JsonPropertyName("ticketId")]
        public int? TicketId { get; set; }
    }
}
