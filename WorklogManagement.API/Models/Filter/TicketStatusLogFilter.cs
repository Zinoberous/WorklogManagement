using System.Text.Json.Serialization;

namespace WorklogManagement.API.Models.Filter
{
    public class TicketStatusLogFilter : IFilter
    {
        [JsonPropertyName("ticketId")]
        public int? TicketId { get; set; }
    }
}
