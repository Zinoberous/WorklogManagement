using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WorklogManagement.API.Models.Filter
{
    public class TicketAttachmentFilter : IFilter
    {
        [JsonPropertyName("ticketId")]
        public int? TicketId { get; set; }

        [JsonPropertyName("name")]
        [StringLength(255)]
        public string? Name { get; set; } = null!;
    }
}
