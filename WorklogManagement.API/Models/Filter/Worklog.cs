using System.Text.Json.Serialization;

namespace WorklogManagement.API.Models.Filter
{
    public partial class WorklogFilter : IFilter
    {
        [JsonPropertyName("date")]
        public DateOnly? Date { get; set; }

        [JsonPropertyName("ticketId")]
        public int? TicketId { get; set; }

        [JsonPropertyName("search")]
        public string? Search { get; set; }
    }
}
