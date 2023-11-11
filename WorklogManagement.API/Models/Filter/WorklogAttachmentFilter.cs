using System.Text.Json.Serialization;

namespace WorklogManagement.API.Models.Filter
{
    public class WorklogAttachmentFilter : IFilter
    {
        [JsonPropertyName("worklogId")]
        public int? WorklogId { get; set; }
    }
}
