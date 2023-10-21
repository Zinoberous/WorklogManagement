using System.Text.Json.Serialization;

namespace WorklogManagement.API.Models.Filter
{
    public class DayFilter : IFilter
    {
        [JsonPropertyName("date")]
        public DateTime? Date { get; set; }

        [JsonPropertyName("isMobile")]
        public bool? IsMobile { get; set; }

        [JsonPropertyName("workloadId")]
        public int? WorkloadId { get; set; }
    }
}
