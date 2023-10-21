using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WorklogManagement.API.Models.Filter
{
    public class WorklogAttachmentFilter : IFilter
    {
        [JsonPropertyName("worklogId")]
        public int? WorklogId { get; set; }

        [JsonPropertyName("name")]
        [StringLength(255)]
        public string? Name { get; set; } = null!;
    }
}
