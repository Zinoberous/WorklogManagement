﻿using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WorklogManagement.API.Interfaces;
using WorklogManagement.API.Models;

namespace WorklogManagement.API.Helper
{
    internal static class RequestHelper
    {
        private static Expression<Func<T, object?>> OrderByExpression<T>(string columnName)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.PropertyOrField(parameter, columnName);
            var conversion = Expression.Convert(property, typeof(object));  // We need to convert it to object, since EF Core requires ThenBy/OrderBy to return IOrderedQueryable<T>
            return Expression.Lambda<Func<T, object?>>(conversion, parameter);
        }

        internal static async Task<Result<TResult, TQuery>> GetAsync<TData, TQuery, TResult>
        (
            IQueryable<TData> items,
            TQuery query,
            Expression<Func<TData, TResult>> select,
            Expression<Func<TData, bool>> where
        )
            where TData : class
            where TQuery : IQuery
            where TResult : class
        {
            var filteredItems = items.Where(where);

            var totalItems = filteredItems.Count();

            if (totalItems == 0)
            {
                return new(query, 0, 0, await filteredItems.Select(select).ToListAsync());
            }

            IOrderedQueryable<TData>? orderedItems = null;

            foreach (var sort in query.Sort.Split(','))
            {
                var desc = sort.EndsWith("desc", StringComparison.InvariantCultureIgnoreCase);

                var column =
                    sort.EndsWith("asc", StringComparison.InvariantCultureIgnoreCase) ||
                    sort.EndsWith("desc", StringComparison.InvariantCultureIgnoreCase)
                        ? string.Join(' ', sort.Split(' ').SkipLast(1))
                        : sort;

                if (orderedItems == null)
                {
                    orderedItems = desc
                        ? filteredItems.OrderByDescending(OrderByExpression<TData>(column))
                        : filteredItems.OrderBy(OrderByExpression<TData>(column));
                }
                else
                {
                    orderedItems = desc
                        ? orderedItems.ThenByDescending(OrderByExpression<TData>(column))
                        : orderedItems.ThenBy(OrderByExpression<TData>(column));
                }
            }

            filteredItems = orderedItems ?? filteredItems;

            var totalPages = query.PageSize == 0 ? 1 : Math.Ceiling((double)totalItems / query.PageSize);

            var pageIndex = query.PageIndex >= totalPages
                    ? totalPages - 1
                    : query.PageIndex;

            var page = query.PageSize == 0
                ? filteredItems
                : filteredItems
                    .Skip((int)(pageIndex * query.PageSize))
                    .Take((int)query.PageSize);

            var result = await page
                .Select(select)
                .ToListAsync();

            return new(query, (uint)totalPages, (uint)totalItems, result);
        }
    }
}
