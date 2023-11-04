using System.Text.Json.Serialization;

namespace WorklogManagement.API.Models.Filter
{
    public class DayFilter : IFilter
    {
        [JsonPropertyName("date")]
        public DateTime? Date { get; set; }

        [JsonPropertyName("from")]
        public DateTime? From { get; set; }

        [JsonPropertyName("to")]
        public DateTime? To { get; set; }

        [JsonPropertyName("isMobile")]
        public bool? IsMobile { get; set; }

        [JsonPropertyName("workloadId")]
        public int? WorkloadId { get; set; }
    }
}
