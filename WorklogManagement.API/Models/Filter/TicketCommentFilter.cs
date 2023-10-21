using System.Text.Json.Serialization;

namespace WorklogManagement.API.Models.Filter
{
    public class TicketCommentFilter : IFilter
    {
        [JsonPropertyName("ticketId")]
        public int? TicketId { get; set; }
    }
}
