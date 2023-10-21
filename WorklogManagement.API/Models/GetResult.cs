using WorklogManagement.API.Models.Data;
using WorklogManagement.API.Models.Filter;

namespace WorklogManagement.API.Models
{
    public class GetResult<TData, TFilter> : GetRequest<TFilter>
        where TData : IData
        where TFilter : IFilter
    {
        public uint TotalItems { get; set; }
        public uint TotalPages { get; set; }
        public IEnumerable<TData> Data { get; set; } = null!;
    }
}
