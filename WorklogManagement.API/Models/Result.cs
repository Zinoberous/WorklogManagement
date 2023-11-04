using WorklogManagement.API.Interfaces;

namespace WorklogManagement.API.Models
{
    public class Result<TData, TQuery>
    where TData : class
    where TQuery : IQuery
    {
        public TQuery Query { get; }

        public uint TotalItems { get; }

        public uint TotalPages { get; }

        // TODO: uri first page

        // TODO: uri last page

        // TODO: uri? next page
        
        // TODO: uri? previous page

        public IEnumerable<TData> Items { get; }

        public Result(TQuery query, IEnumerable<TData> items, uint totalItems)
        {
            Query = query;
            Items = items;

            TotalItems = totalItems;
            TotalPages = query.PageSize == 0 ? 1 : (uint)Math.Ceiling((double)TotalItems / query.PageSize);
        }
    }
}
