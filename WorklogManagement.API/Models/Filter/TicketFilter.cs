using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WorklogManagement.API.Models.Filter
{
    public class TicketFilter : IFilter
    {
        [JsonPropertyName("refId")]
        public int? RefId { get; set; }

        [JsonPropertyName("title")]
        [StringLength(255)]
        public string? Title { get; set; } = null!;

        [JsonPropertyName("statusId")]
        public int? StatusId { get; set; }
    }
}
