using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WorklogManagement.API.Enums;

namespace WorklogManagement.API.Models.Filter
{
    public class TicketFilter : IFilter
    {
        [JsonPropertyName("refId")]
        public int? RefId { get; set; }

        [JsonPropertyName("title")]
        [StringLength(255)]
        public string? Title { get; set; } = null!;

        [JsonPropertyName("status")]
        public IEnumerable<string>? Status { get; set; }

        internal IEnumerable<TicketStatus>? StatusEnum => Status?.Select(x => Enum.Parse<TicketStatus>(x));
    }
}
