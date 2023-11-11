using System.Text.Json.Serialization;

namespace WorklogManagement.API.Models.Filter
{
    public class TicketAttachmentFilter : IFilter
    {
        [JsonPropertyName("ticketId")]
        public int? TicketId { get; set; }
    }
}
