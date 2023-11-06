using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WorklogManagement.API.Models.Filter
{
    public class TicketCommentAttachmentFilter : IFilter
    {
        [JsonPropertyName("ticketCommentId")]
        public int? TicketCommentId { get; set; }

        [JsonPropertyName("name")]
        [StringLength(255)]
        public string? Name { get; set; } = null!;
    }
}
