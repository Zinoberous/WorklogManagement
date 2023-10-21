using WorklogManagement.API.Models.Filter;

namespace WorklogManagement.API.Models
{
    public class GetRequest<TFilter> where TFilter : IFilter
    {
        public IEnumerable<Sort> Sorting { get; set; } = new List<Sort>();
        public uint PageSize { get; set; } = 0;
        public uint Page { get; set; } = 0;
        public TFilter? Filter { get; set; } = default;
    }

    public class Sort
    {
        public string Column { get; set; } = "Id";
        public bool Desc { get; set; } = false;
    }
}
