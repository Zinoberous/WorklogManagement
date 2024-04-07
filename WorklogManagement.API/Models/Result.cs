using WorklogManagement.API.Interfaces;

namespace WorklogManagement.API.Models
{
    public class Result<TData, TQuery>
    where TData : class
    where TQuery : IQuery
    {
        public TQuery Query { get; }

        // TODO: uri first page

        // TODO: uri last page

        // TODO: uri? next page

        // TODO: uri? previous page

        public uint TotalPages { get; }

        public uint TotalItems { get; }

        public IEnumerable<TData> Items { get; }

        public Result(TQuery query, uint totalPages, uint totalItems, IEnumerable<TData> items)
        {
            Query = query;
            TotalPages = totalPages;
            TotalItems = totalItems;
            Items = items;
        }
    }
}
