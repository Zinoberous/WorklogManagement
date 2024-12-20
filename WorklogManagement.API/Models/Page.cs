using System.Linq.Dynamic.Core;
using System.Text;
using Shd = WorklogManagement.Shared.Models;

namespace WorklogManagement.API.Models;

public record Page<TData> : Shd.Page<TData>
    where TData : Shd.IDataModel
{

}

public record Page
{
    internal static IQueryable<T> GetQuery<T>(
        IQueryable<T> items,
        out uint totalItems,
        out uint totalPages,
        ref uint pageIndex,
        uint pageSize,
        string? sortBy,
        string? filter,
        IReadOnlyDictionary<string, string> propertyNameMappings)
    {
        if (!string.IsNullOrWhiteSpace(filter))
        {
            filter = TranslateFilter(filter, propertyNameMappings);

            items = items.Where(filter);
        }

        totalItems = (uint)items.Count();

        if (totalItems == 0)
        {
            totalPages = 1;
            pageIndex = 0;

            return items;
        }

        totalPages = (uint)(pageSize == 0 ? 1 : Math.Ceiling((double)totalItems / pageSize));

        pageIndex = pageIndex >= totalPages
                ? totalPages - 1
                : pageIndex;

        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            sortBy = TranslatePropertyNames(sortBy, propertyNameMappings);

            items = items.OrderBy(sortBy);
        }

        var page = pageSize == 0
            ? items
            : items
                .Skip((int)(pageIndex * pageSize))
                .Take((int)pageSize);

        return page;
    }

    private static string TranslatePropertyNames(string s, IReadOnlyDictionary<string, string> propertyNameMapping)
    {
        foreach (var map in propertyNameMapping)
        {
            s = s.Replace(map.Key, map.Value, StringComparison.OrdinalIgnoreCase);
        }

        return s;
    }

    private static string TranslateFilter(string filter, IReadOnlyDictionary<string, string> propertyNameMapping)
    {
        StringBuilder result = new();

        StringBuilder buffer = new();

        var insideString = false;
        var stringQuotes = '\0';

        char? prev = null;
        foreach (var c in filter)
        {
            // string start
            if ((c == '\'' || c == '\"') && !insideString)
            {
                result.Append(TranslatePropertyNames(buffer.ToString(), propertyNameMapping));

                insideString = true;
                stringQuotes = c;

                buffer.Clear();
            }
            // string end
            else if (c == stringQuotes && insideString && prev != '\\')
            {
                result.Append($"{c}{buffer}{c}");

                insideString = false;
                stringQuotes = '\0';

                buffer.Clear();
            }
            else
            {
                buffer.Append(c);
            }

            prev = c;
        }

        if (!insideString)
        {
            result.Append(TranslatePropertyNames(buffer.ToString(), propertyNameMapping));
        }

        return result.ToString();
    }
}
