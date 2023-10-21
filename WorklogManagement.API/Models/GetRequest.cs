using System.Text.Json.Serialization;
using WorklogManagement.API.Models.Filter;

namespace WorklogManagement.API.Models
{
    public class GetRequest<TFilter> where TFilter : IFilter
    {
        [JsonPropertyName("sorting")]
        public IEnumerable<Sort> Sorting { get; set; } = new List<Sort>();

        [JsonPropertyName("pageSize")]
        public uint PageSize { get; set; } = 0;

        [JsonPropertyName("page")]
        public uint Page { get; set; } = 0;

        [JsonPropertyName("filter")]
        public TFilter? Filter { get; set; } = default;
    }

    public class Sort
    {
        [JsonPropertyName("column")]
        public string Column { get; set; } = "Id";

        [JsonPropertyName("desc")]
        public bool Desc { get; set; } = false;
    }
}
