using System.Text.Json.Serialization;
using WorklogManagement.API.Models.Data;
using WorklogManagement.API.Models.Filter;

namespace WorklogManagement.API.Models
{
    public class GetResult<TData, TFilter> : GetRequest<TFilter>
        where TData : IData
        where TFilter : IFilter
    {
        [JsonPropertyName("totalItems")]
        public uint TotalItems { get; set; }

        [JsonPropertyName("totalPages")]
        public uint TotalPages { get; set; }

        [JsonPropertyName("data")]
        public IEnumerable<TData> Data { get; set; } = null!;
    }
}
